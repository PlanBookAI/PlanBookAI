import json
import re  # <<< THÊM IMPORT NÀY
from fastapi import APIRouter, UploadFile, File, Form, Depends, HTTPException, Body
from sqlalchemy.orm import Session
from pydantic import BaseModel

from app.db.session import get_db
from app.schemas.lesson_plan import LessonPlanOut
from app.services import pdf_service, gemini_service
from app.models.lesson_plan import LessonPlan

router = APIRouter()

# --- Các PROMPT TEMPLATE giữ nguyên như cũ ---

JSON_PROMPT_TEMPLATE_WITH_CONTEXT = """
**YÊU CẦU NHIỆM VỤ:**
Phân tích tài liệu và yêu cầu người dùng để tạo nội dung cho một giáo án.
Sau đó, trả về kết quả dưới dạng một đối tượng JSON duy nhất và hợp lệ.

**RÀNG BUỘC ĐỊNH DẠNG JSON (BẮT BUỘC):**
Đối tượng JSON phải có cấu trúc như sau:
{{
  "tieu_de": "Tiêu đề của giáo án",
  "noi_dung": "Toàn bộ nội dung chi tiết của giáo án, sử dụng định dạng Markdown với các ký tự xuống dòng \\n để trình bày rõ ràng (ví dụ: các mục, gạch đầu dòng)."
}}

**RÀNG BUỘC ĐẦU RA (RẤT QUAN TRỌNG):**
1.  **CHỈ** trả về đối tượng JSON.
2.  **KHÔNG** được bao bọc JSON trong các khối mã như ```json ... ```.
3.  **KHÔNG** thêm bất kỳ văn bản nào khác trước hoặc sau đối tượng JSON.

**NGỮ CẢNH TỪ TÀI LIỆU:**
---
{context_text}
---

**YÊU CẦU CỤ THỂ CỦA NGƯỜI DÙNG:**
---
{user_prompt}
---
"""

JSON_PROMPT_TEMPLATE_WITHOUT_CONTEXT = """
**YÊU CẦU NHIỆM VỤ:**
Dựa vào yêu cầu của người dùng để tạo nội dung cho một giáo án.
Sau đó, trả về kết quả dưới dạng một đối tượng JSON duy nhất và hợp lệ.

**RÀNG BUỘC ĐỊNH DẠNG JSON (BẮT BUỘC):**
Đối tượng JSON phải có cấu trúc như sau:
{{
  "tieu_de": "Tiêu đề của giáo án",
  "noi_dung": "Toàn bộ nội dung chi tiết của giáo án, sử dụng định dạng Markdown với các ký tự xuống dòng \\n để trình bày rõ ràng (ví dụ: các mục, gạch đầu dòng)."
}}

**RÀNG BUỘC ĐẦU RA (RẤT QUAN TRỌNG):**
1.  **CHỈ** trả về đối tượng JSON.
2.  **KHÔNG** được bao bọc JSON trong các khối mã như ```json ... ```.
3.  **KHÔNG** thêm bất kỳ văn bản nào khác trước hoặc sau đối tượng JSON.

**YÊU CẦU CỤ THỂ CỦA NGƯỜI DÙNG:**
---
{user_prompt}
---
"""


class PromptInput(BaseModel):
    prompt: str


# --- HÀM TRỢ GIÚP ĐỂ TÁI SỬ DỤNG LOGIC XỬ LÝ JSON ---
def process_and_save_lesson_plan(
        db: Session,
        raw_response: str,
        source_type: str,
        prompt: str
) -> LessonPlan:
    """
    Hàm này xử lý phản hồi thô từ AI, phân tích JSON,
    và lưu giáo án vào database.
    """
    try:
        # <<< THAY ĐỔI LỚN BẮT ĐẦU TỪ ĐÂY >>>
        # Sử dụng regex để tìm khối JSON, kể cả khi có lỗi cú pháp nhỏ
        # re.DOTALL cho phép '.' khớp với cả ký tự xuống dòng
        match = re.search(r'\{.*\}', raw_response, re.DOTALL)

        if not match:
            # Nếu không tìm thấy bất kỳ khối nào bắt đầu bằng { và kết thúc bằng }
            raise ValueError("Không tìm thấy đối tượng JSON trong phản hồi của AI.")

        json_string = match.group(0)

        # Bây giờ mới thử phân tích chuỗi JSON đã được trích xuất
        data = json.loads(json_string)
        # <<< KẾT THÚC THAY ĐỔI LỚN >>>

        lesson_title = data.get("tieu_de", "Không có tiêu đề")
        lesson_content = data.get("noi_dung")

        if not lesson_content:
            raise ValueError("Phản hồi JSON của AI thiếu key 'noi_dung' bắt buộc.")

        final_content = f"# {lesson_title}\n\n{lesson_content}"

    except (json.JSONDecodeError, ValueError) as e:
        # Ghi log lỗi và dữ liệu thô để debug
        print(f"Lỗi phân tích JSON từ AI: {e}")
        print(f"Dữ liệu thô nhận được:\n---\n{raw_response}\n---")
        # Ném ra HTTPException để trả về lỗi 502 cho client
        raise HTTPException(
            status_code=502,
            detail="AI đã không trả về dữ liệu theo định dạng JSON hợp lệ. Vui lòng thử lại sau."
        )

    # Lưu kết quả vào Database
    db_obj = LessonPlan(
        source_type=source_type,
        prompt=prompt,
        content=final_content
    )
    db.add(db_obj)
    db.commit()
    db.refresh(db_obj)

    return db_obj


@router.post("/tao-giao-an-tu-pdf", response_model=LessonPlanOut, summary="Tạo giáo án từ file PDF và prompt")
async def create_lesson_plan_from_pdf(
        db: Session = Depends(get_db),
        pdf_file: UploadFile = File(..., description="File giáo trình định dạng PDF"),
        prompt: str = Form(..., description="Yêu cầu chi tiết để tạo giáo án")
):
    pdf_content = await pdf_service.extract_text_from_pdf(pdf_file)
    if not pdf_content.strip():
        raise HTTPException(status_code=400, detail="Không thể trích xuất nội dung từ file PDF hoặc file PDF rỗng.")

    full_prompt = JSON_PROMPT_TEMPLATE_WITH_CONTEXT.format(
        context_text=pdf_content,
        user_prompt=prompt
    )
    raw_response = gemini_service.generate_content(full_prompt)

    # Gọi hàm trợ giúp để xử lý và lưu
    return process_and_save_lesson_plan(db, raw_response, "pdf", prompt)


@router.post("/tao-giao-an-tu-prompt", response_model=LessonPlanOut, summary="Tạo giáo án chỉ từ prompt")
async def create_lesson_plan_from_prompt(
        payload: PromptInput,
        db: Session = Depends(get_db)
):
    full_prompt = JSON_PROMPT_TEMPLATE_WITHOUT_CONTEXT.format(
        user_prompt=payload.prompt
    )
    raw_response = gemini_service.generate_content(full_prompt)

    # Gọi hàm trợ giúp để xử lý và lưu
    return process_and_save_lesson_plan(db, raw_response, "prompt", payload.prompt)