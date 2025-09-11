# File: app/api/v1/endpoints/exams.py

import json
import re
import random  # <<< THÊM IMPORT NÀY
from fastapi import APIRouter, UploadFile, File, Form, Depends, HTTPException
from sqlalchemy.orm import Session
from pydantic import ValidationError

from app.db.session import get_db
from app.services import pdf_service, gemini_service
from app.schemas.exam import Exam, ExamDifficulty

router = APIRouter()

# --- PROMPT TEMPLATE KHÔNG CẦN THAY ĐỔI ---
# Chúng ta không yêu cầu AI tạo mã đề, server sẽ tự làm việc này.
JSON_PROMPT_TEMPLATE_FOR_EXAM = """
**YÊU CẦU NHIỆM VỤ:**
Dựa vào nội dung tài liệu được cung cấp, hãy tạo một bộ đề thi trắc nghiệm.

**THAM SỐ:**
- Số lượng câu hỏi: {num_questions}
- Độ khó: {difficulty}

**RÀNG BUỘC ĐỊNH DẠNG JSON (TUYỆT ĐỐI BẮT BUỘC):**
Trả về MỘT đối tượng JSON duy nhất và hợp lệ có cấu trúc chính xác như sau:
{{
  "title": "Tiêu đề phù hợp cho đề thi (ví dụ: Đề kiểm tra 15 phút - Chủ đề Blockchain)",
  "questions": [
    {{
      "question_number": 1,
      "question_text": "Nội dung đầy đủ của câu hỏi số 1?",
      "options": {{ "A": "...", "B": "...", "C": "...", "D": "..." }},
      "correct_answer": "A"
    }}
  ]
}}

**RÀNG BUỘC NỘI DUNG (RẤT QUAN TRỌNG):**
1.  **CHỈ** trả về đối tượng JSON. KHÔNG thêm bất kỳ lời chào, giải thích, hay ghi chú nào.
2.  Phải tạo **CHÍNH XÁC** {num_questions} câu hỏi.
3.  Mỗi câu hỏi phải có **ĐÚNG 4** lựa chọn, được gán nhãn "A", "B", "C", "D".
4.  Mỗi câu hỏi phải có một "correct_answer" là một trong các key "A", "B", "C", "D".
5.  Nội dung câu hỏi và lựa chọn phải dựa trên tài liệu đã cho.

**NỘI DUNG TÀI LIỆU:**
---
{context_text}
---
"""


@router.post("/tao-de-thi", response_model=Exam, summary="Tạo bộ đề thi từ file PDF")
async def create_exam_from_pdf(
        pdf_file: UploadFile = File(..., description="File giáo trình định dạng PDF"),
        num_questions: int = Form(..., gt=0, description="Số câu hỏi cần tạo"),
        difficulty: ExamDifficulty = Form(..., description="Độ khó của đề thi (Dễ, Trung bình, Khó)")
):
    pdf_content = await pdf_service.extract_text_from_pdf(pdf_file)
    if not pdf_content.strip():
        raise HTTPException(status_code=400, detail="Không thể trích xuất nội dung từ file PDF hoặc file PDF rỗng.")

    full_prompt = JSON_PROMPT_TEMPLATE_FOR_EXAM.format(
        num_questions=num_questions,
        difficulty=difficulty.value,
        context_text=pdf_content
    )
    raw_response = gemini_service.generate_content(full_prompt)

    try:
        match = re.search(r'\{.*\}', raw_response, re.DOTALL)
        if not match:
            raise ValueError("Không tìm thấy đối tượng JSON trong phản hồi của AI.")

        json_string = match.group(0)
        data = json.loads(json_string)

        # 1. Tạo mã đề ngẫu nhiên
        random_exam_code = random.randint(100, 999)

        # 2. Thêm mã đề vào dữ liệu nhận được từ AI
        data['exam_code'] = random_exam_code

        # 3. Validate toàn bộ dữ liệu (bao gồm cả mã đề vừa thêm)
        exam_data = Exam.model_validate(data)

        if len(exam_data.questions) != num_questions:
            raise ValueError(
                f"AI đã tạo {len(exam_data.questions)} câu hỏi, không khớp với yêu cầu là {num_questions} câu.")

        return exam_data

    except (json.JSONDecodeError, ValidationError, ValueError) as e:
        print(f"Lỗi xử lý phản hồi từ AI: {e}")
        print(f"Dữ liệu thô nhận được:\n---\n{raw_response}\n---")
        raise HTTPException(
            status_code=502,
            detail=f"AI không trả về dữ liệu hợp lệ hoặc không tuân thủ ràng buộc. Chi tiết: {str(e)}"
        )