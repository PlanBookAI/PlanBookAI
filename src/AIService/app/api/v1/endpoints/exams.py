import json
import re
from fastapi import APIRouter, UploadFile, File, Form, Depends, HTTPException
from pydantic import BaseModel, Field, field_validator
from typing import List
from enum import Enum

from app.services import pdf_service, gemini_service

router = APIRouter()

class ExamDifficulty(str, Enum):
    EASY = "Dễ"
    MEDIUM = "Trung bình"
    HARD = "Khó"

# --- Pydantic Models (Không thay đổi) ---
class LuaChon(BaseModel):
    noiDung: str
    laDapAnDung: bool


class CauHoiTaoMoi(BaseModel):
    noiDung: str = Field(..., description="Nội dung đầy đủ của câu hỏi")
    giaiThich: str = Field(..., description="Giải thích chi tiết cho đáp án")
    dapAnDung: str = Field(..., description="Đáp án đúng (chỉ là key, ví dụ: 'A')")
    luaChons: List[LuaChon] = Field(..., description="Danh sách 4 lựa chọn")

    @field_validator('luaChons')
    def check_options(cls, v):
        if len(v) != 4:
            raise ValueError('Phải có đúng 4 lựa chọn')

        correct_answers_count = sum(1 for choice in v if choice.laDapAnDung)
        if correct_answers_count != 1:
            raise ValueError('Phải có chính xác một lựa chọn được đánh dấu laDapAnDung = true')
        return v

    @field_validator('dapAnDung')
    def check_correct_answer_key(cls, v):
        if v not in ['A', 'B', 'C', 'D']:
            raise ValueError("dapAnDung phải là 'A', 'B', 'C', hoặc 'D'")
        return v


# --- Prompt Template (ĐÃ THÊM RÀNG BUỘC) ---
JSON_PROMPT_TEMPLATE_FOR_QUESTION_BANK = """
**YÊU CẦU NHIỆM VỤ:**
Dựa vào nội dung tài liệu, hãy tạo một bộ câu hỏi trắc nghiệm.
Kết quả trả về phải là một MẢNG JSON (JSON array), trong đó mỗi phần tử là một đối tượng câu hỏi.

**THAM SỐ:**
- Số lượng câu hỏi: {num_questions}
- Độ khó: {difficulty}

**RÀNG BUỘC ĐỊNH DẠNG JSON (TUYỆT ĐỐI BẮT BUỘC):**
Trả về MỘT MẢNG JSON duy nhất và hợp lệ có cấu trúc chính xác như sau:
[
  {{
    "noiDung": "Nội dung đầy đủ của câu hỏi số 1?",
    "giaiThich": "Giải thích chi tiết và rõ ràng tại sao đáp án lại đúng. (KHÔNG ĐƯỢC NHẮC ĐẾN TÀI LIỆU NGUỒN)",
    "dapAnDung": "A",
    "luaChons": [
      {{ "noiDung": "Nội dung lựa chọn A", "laDapAnDung": true }},
      {{ "noiDung": "Nội dung lựa chọn B", "laDapAnDung": false }},
      {{ "noiDung": "Nội dung lựa chọn C", "laDapAnDung": false }},
      {{ "noiDung": "Nội dung lựa chọn D", "laDapAnDung": false }}
    ]
  }},
  {{
    "noiDung": "Nội dung đầy đủ của câu hỏi số 2?",
    "giaiThich": "Giải thích cho câu hỏi 2...",
    "dapAnDung": "C",
    "luaChons": [
      {{ "noiDung": "...", "laDapAnDung": false }},
      {{ "noiDung": "...", "laDapAnDung": false }},
      {{ "noiDung": "...", "laDapAnDung": true }},
      {{ "noiDung": "...", "laDapAnDung": false }}
    ]
  }}
]

**RÀNG BUỘC NỘI DUNG (RẤT QUAN TRỌNG):**
1.  **CHỈ** trả về mảng JSON. KHÔNG thêm bất kỳ văn bản, tiêu đề, hay ghi chú nào khác.
2.  **KHÔNG** bao bọc mảng JSON trong một đối tượng cha.
3.  **TUYỆT ĐỐI KHÔNG ĐƯỢC ĐỀ CẬP** đến các từ khóa như "tài liệu nguồn", "file PDF", "ngữ cảnh đã cho", "theo nội dung trên" hoặc bất kỳ từ nào ám chỉ đến nguồn dữ liệu được cung cấp, trong cả trường "noiDung" và "giaiThich". Nội dung phải là độc lập.
4.  Phải tạo **CHÍNH XÁC** {num_questions} câu hỏi.
5.  Mỗi câu hỏi phải có **ĐÚNG 4** lựa chọn.
6.  Mỗi câu hỏi phải có **CHÍNH XÁC 1** lựa chọn có `"laDapAnDung": true`.
7.  Trường `"dapAnDung"` (A, B, C, D) phải khớp với lựa chọn có `"laDapAnDung": true`.
8.  Nội dung câu hỏi, lựa chọn và giải thích phải dựa trên tài liệu đã cho.

**NỘI DUNG TÀI LIỆU:**
---
{context_text}
---
"""


# --- Endpoint (Không thay đổi) ---
@router.post("/tao-danh-sach-cau-hoi", response_model=List[CauHoiTaoMoi],
             summary="Tạo một danh sách câu hỏi từ file PDF để thêm vào ngân hàng câu hỏi")
async def create_question_list_from_pdf(
        pdf_file: UploadFile = File(..., description="File giáo trình định dạng PDF"),
        num_questions: int = Form(..., gt=0, description="Số câu hỏi cần tạo"),
        difficulty: ExamDifficulty = Form(..., description="Độ khó của các câu hỏi (Dễ, Trung bình, Khó)")
):
    pdf_content = await pdf_service.extract_text_from_pdf(pdf_file)
    if not pdf_content.strip():
        raise HTTPException(status_code=400, detail="Không thể trích xuất nội dung từ file PDF hoặc file PDF rỗng.")

    full_prompt = JSON_PROMPT_TEMPLATE_FOR_QUESTION_BANK.format(
        num_questions=num_questions,
        difficulty=difficulty.value,
        context_text=pdf_content
    )
    raw_response = gemini_service.generate_content(full_prompt)

    try:
        match = re.search(r'\[.*\]', raw_response, re.DOTALL)
        if not match:
            raise ValueError("Không tìm thấy mảng JSON trong phản hồi của AI.")

        json_string = match.group(0)
        questions_data = json.loads(json_string)

        if not isinstance(questions_data, list):
            raise ValueError("Phản hồi của AI không phải là một danh sách (mảng).")

        if len(questions_data) != num_questions:
            raise ValueError(f"AI đã tạo {len(questions_data)} câu hỏi, không khớp với yêu cầu là {num_questions} câu.")

        validated_questions = [CauHoiTaoMoi.model_validate(q) for q in questions_data]

        return validated_questions

    except (json.JSONDecodeError, ValueError) as e:
        print(f"Lỗi xử lý phản hồi từ AI: {e}")
        print(f"Dữ liệu thô nhận được:\n---\n{raw_response}\n---")
        raise HTTPException(
            status_code=502,
            detail=f"AI không trả về dữ liệu hợp lệ hoặc không tuân thủ ràng buộc. Chi tiết: {str(e)}"
        )