import google.generativeai as genai
from app.core.config import settings
import logging

# Cấu hình logging cơ bản
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

genai.configure(api_key=settings.GEMINI_API_KEY)

# Cấu hình model
generation_config = {
  "temperature": 0.7,
  "top_p": 1,
  "top_k": 1,
  "max_output_tokens": 8192,
}

safety_settings = [
  {"category": "HARM_CATEGORY_HARASSMENT", "threshold": "BLOCK_MEDIUM_AND_ABOVE"},
  {"category": "HARM_CATEGORY_HATE_SPEECH", "threshold": "BLOCK_MEDIUM_AND_ABOVE"},
  {"category": "HARM_CATEGORY_SEXUALLY_EXPLICIT", "threshold": "BLOCK_MEDIUM_AND_ABOVE"},
  {"category": "HARM_CATEGORY_DANGEROUS_CONTENT", "threshold": "BLOCK_MEDIUM_AND_ABOVE"},
]

model = genai.GenerativeModel(model_name="gemini-2.5-flash",
                              generation_config=generation_config,
                              safety_settings=safety_settings)

def generate_content(prompt: str) -> str:
    try:
        logger.info("Đang gửi yêu cầu đến Gemini API...")
        response = model.generate_content(prompt)
        # Thêm kiểm tra nếu Gemini trả về mà không có text
        if not response.text:
            logger.warning("Gemini API trả về phản hồi rỗng.")
            raise ValueError("Empty response from Gemini API")
        logger.info("Nhận phản hồi thành công từ Gemini API.")
        return response.text
    except Exception as e:
        # Đây là phần quan trọng nhất: In ra lỗi thực tế
        logger.error(f"LỖI KHI GỌI GEMINI API: {e}", exc_info=True)
        # Thay vì trả về chuỗi lỗi, chúng ta sẽ raise exception để API endpoint xử lý
        # Điều này sẽ giúp API trả về mã lỗi 500 thay vì 200, phản ánh đúng bản chất vấn đề.
        raise e