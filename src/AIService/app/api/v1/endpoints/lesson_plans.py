import json
import re
from fastapi import APIRouter, UploadFile, File, Form, Depends, HTTPException, Body
from pydantic import BaseModel, Field
from typing import Dict, List, Any

from app.services import pdf_service, gemini_service

router = APIRouter()


# --- CÁC MODEL PYDANTIC ---
# Class MoTaChiTiet không thay đổi
class MoTaChiTiet(BaseModel):
    OnDinhToChuc: str = Field(..., description="Nội dung ổn định tổ chức lớp học")
    KiemTraBaiCu: str = Field(..., description="Câu hỏi hoặc nội dung kiểm tra bài cũ")
    KhoiDong: str = Field(..., description="Hoạt động khởi động, dẫn dắt vào bài mới")
    HinhThanhKienThuc: Dict[str, Any] = Field(...,
                                              description="Đối tượng chứa các hoạt động hình thành kiến thức mới, ví dụ: 'NoiDung1': {'TieuDe': '...', 'HoatDongGV': '...'}")
    LuyenTapVanDung: Dict[str, Any] = Field(..., description="Các bài tập và hoạt động luyện tập, vận dụng")
    DanhGia: Dict[str, Any] = Field(..., description="Hình thức và tiêu chí đánh giá")
    TaiNguyenHocLieu: List[str] = Field(..., description="Danh sách các tài nguyên, học liệu cần chuẩn bị")
    HuongDanVeNha: str = Field(..., description="Nội dung dặn dò, hướng dẫn học sinh tự học ở nhà")
    DieuChinhBoSung: str | None = Field(None, description="Ghi chú về các điều chỉnh, bổ sung nếu có")


class LessonPlanAIResponse(BaseModel):
    TieuDe: str = Field(..., description="Tiêu đề của giáo án")
    MucTieu: str = Field(..., description="Mục tiêu bài học (kiến thức, kỹ năng, thái độ)")

    # <<< SỬA LỖI Ở ĐÂY >>>
    # 1. Đổi tên thuộc tính thành snake_case: `mo_ta_chi_tiet`
    # 2. Dùng `alias="MoTaChiTiet"` để đảm bảo JSON output vẫn có key là "MoTaChiTiet"
    mo_ta_chi_tiet: MoTaChiTiet = Field(..., alias="MoTaChiTiet",
                                        description="Nội dung chi tiết có cấu trúc của giáo án")


# --- PROMPT TEMPLATE (Không cần thay đổi) ---
JSON_PROMPT_TEMPLATE_STRUCTURED = """
**YÊU CẦU NHIỆM VỤ:**
Dựa vào tài liệu và yêu cầu người dùng, hãy tạo nội dung chi tiết cho một giáo án.
Sau đó, trả về kết quả dưới dạng MỘT đối tượng JSON duy nhất, hợp lệ, và có cấu trúc phức tạp theo đúng yêu cầu dưới đây.

**RÀNG BUỘC ĐỊNH DẠNG JSON (TUYỆT ĐỐI BẮT BUỘC):**
Đối tượng JSON phải có cấu trúc chính xác như sau:
{{
  "TieuDe": "Tiêu đề phù hợp cho giáo án",
  "MucTieu": "Trình bày các mục tiêu chính của bài học dưới dạng text, sử dụng gạch đầu dòng và ký tự xuống dòng \\n.",
  "MoTaChiTiet": {{
    "OnDinhToChuc": "Nội dung cho phần ổn định tổ chức (ví dụ: 'Kiểm tra sĩ số, ổn định lớp học. Thời gian: 2 phút').",
    "KiemTraBaiCu": "Nội dung cho phần kiểm tra bài cũ (ví dụ: 'Câu hỏi: Trình bày cấu tạo vỏ electron của nguyên tử. Thời gian: 5 phút').",
    "KhoiDong": "Nội dung cho hoạt động khởi động, tạo hứng thú (ví dụ: 'Chiếu video về mô hình hệ mặt trời và đặt câu hỏi liên tưởng đến cấu trúc nguyên tử.').",
    "HinhThanhKienThuc": {{
      "NoiDung1": {{
        "TieuDe": "Tiêu đề của hoạt động/nội dung chính thứ nhất",
        "ThoiLuong": "Thời lượng dự kiến (ví dụ: 15 phút)",
        "HoatDongGV": "Mô tả các hoạt động của giáo viên.",
        "HoatDongHS": "Mô tả các hoạt động của học sinh.",
        "KienThucCanNho": "Nội dung kiến thức cốt lõi học sinh cần ghi nhớ."
      }},
      "NoiDung2": {{
        "TieuDe": "Tiêu đề của hoạt động/nội dung chính thứ hai",
        "ThoiLuong": "...",
        "HoatDongGV": "...",
        "HoatDongHS": "...",
        "KienThucCanNho": "..."
      }}
    }},
    "LuyenTapVanDung": {{
      "BaiTapNhanh": [
        "Nội dung câu hỏi hoặc bài tập nhanh 1",
        "Nội dung câu hỏi hoặc bài tập nhanh 2"
      ],
      "BaiTapNangCao": "Nội dung bài tập vận dụng nâng cao (nếu có)."
    }},
    "DanhGia": {{
      "HinhThuc": "Mô tả hình thức đánh giá (ví dụ: 'Quan sát, câu hỏi nhanh, phiếu bài tập').",
      "TieuChi": [
        "Tiêu chí đánh giá 1 (ví dụ: 'Học sinh nêu đúng khái niệm...')",
        "Tiêu chí đánh giá 2 (ví dụ: 'Học sinh làm đúng 3/5 bài tập...')"
      ]
    }},
    "TaiNguyenHocLieu": [
      "Liệt kê tài nguyên thứ nhất (ví dụ: 'Sách giáo khoa Hóa học 10')",
      "Liệt kê tài nguyên thứ hai (ví dụ: 'Video mô phỏng chuyển động electron')"
    ],
    "HuongDanVeNha": "Nội dung dặn dò và hướng dẫn về nhà.",
    "DieuChinhBoSung": "Ghi chú về các khả năng cần điều chỉnh trong quá trình dạy."
  }}
}}

**RÀNG BUỘC ĐẦU RA (RẤT QUAN TRỌNG):**
1.  **CHỈ** trả về đối tượng JSON.
2.  **KHÔNG** bao bọc JSON trong các khối mã như ```json ... ```.
3.  **KHÔNG** thêm bất kỳ văn bản nào khác trước hoặc sau đối tượng JSON.

**NGỮ CẢNH TỪ TÀI LIỆU (nếu có):**
---
{context_text}
---

**YÊU CẦU CỤ THỂ CỦA NGƯỜI DÙNG:**
---
{user_prompt}
---
"""


# --- CÁC HÀM VÀ ENDPOINTS (Không cần thay đổi) ---
def process_ai_response(raw_response: str) -> dict:
    try:
        match = re.search(r'\{.*\}', raw_response, re.DOTALL)
        if not match:
            raise ValueError("Không tìm thấy đối tượng JSON trong phản hồi của AI.")

        json_string = match.group(0)
        data = json.loads(json_string)

        # Pydantic sẽ tự động xử lý alias ở đây
        validated_data = LessonPlanAIResponse.model_validate(data)
        # model_dump(by_alias=True) sẽ đảm bảo JSON key là "MoTaChiTiet"
        return validated_data.model_dump(by_alias=True)

    except (json.JSONDecodeError, ValueError) as e:
        print(f"Lỗi phân tích hoặc validate JSON từ AI: {e}")
        print(f"Dữ liệu thô nhận được:\n---\n{raw_response}\n---")
        raise HTTPException(
            status_code=502,
            detail="AI đã không trả về dữ liệu theo định dạng JSON hợp lệ hoặc đầy đủ. Vui lòng thử lại sau."
        )


@router.post("/tao-giao-an-chi-tiet", response_model=LessonPlanAIResponse,
             summary="Tạo nội dung giáo án có cấu trúc chi tiết")
async def create_structured_lesson_plan(
        pdf_file: UploadFile = File(None, description="File giáo trình định dạng PDF (tùy chọn)"),
        prompt: str = Form(..., description="Yêu cầu chi tiết để tạo giáo án")
):
    context_text = "Không có"
    if pdf_file:
        context_text = await pdf_service.extract_text_from_pdf(pdf_file)
        if not context_text.strip():
            raise HTTPException(status_code=400, detail="Không thể trích xuất nội dung từ file PDF hoặc file PDF rỗng.")

    full_prompt = JSON_PROMPT_TEMPLATE_STRUCTURED.format(
        context_text=context_text,
        user_prompt=prompt
    )

    raw_response = gemini_service.generate_content(full_prompt)

    structured_content = process_ai_response(raw_response)

    return structured_content