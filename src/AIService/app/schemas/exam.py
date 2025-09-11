from pydantic import BaseModel, Field, field_validator
from typing import Dict, List
from enum import Enum


# Enum để định nghĩa và giới hạn các lựa chọn độ khó
class ExamDifficulty(str, Enum):
    EASY = "Dễ"
    MEDIUM = "Trung bình"
    HARD = "Khó"


# Mô hình cho một câu hỏi trắc nghiệm
class Question(BaseModel):
    question_number: int = Field(..., description="Số thứ tự câu hỏi")
    question_text: str = Field(..., description="Nội dung câu hỏi")
    options: Dict[str, str] = Field(..., description="Các lựa chọn A, B, C, D")
    correct_answer: str = Field(..., description="Đáp án đúng (chỉ là key, ví dụ: 'A')")

    @field_validator('options')
    def check_options(cls, v):
        if len(v) != 4 or not all(key in ['A', 'B', 'C', 'D'] for key in v.keys()):
            raise ValueError('Phải có đúng 4 lựa chọn là A, B, C, D')
        return v

    @field_validator('correct_answer')
    def check_correct_answer(cls, v, values):
        # 'values.data' chứa dữ liệu đã được validate của các trường trước đó
        if 'options' in values.data and v not in values.data['options']:
            raise ValueError('Đáp án đúng phải là một trong các key của lựa chọn (A, B, C, D)')
        return v


# Mô hình cho toàn bộ đề thi (đây là output của API)
class Exam(BaseModel):
    exam_code: int = Field(..., description="Mã đề thi ngẫu nhiên gồm 3 chữ số")
    title: str = Field(..., description="Tiêu đề của đề thi")
    questions: List[Question] = Field(..., description="Danh sách các câu hỏi")

    @field_validator('exam_code')
    def check_exam_code_range(cls, v):
        if not (100 <= v <= 999):
            raise ValueError('Mã đề phải là một số trong khoảng từ 100 đến 999')
        return v