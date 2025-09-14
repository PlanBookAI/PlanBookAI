from fastapi import APIRouter
from app.api.v1.endpoints import lesson_plans, exams # <<< THÊM `exams` VÀO ĐÂY

api_router = APIRouter()

# Router cho giáo án
api_router.include_router(lesson_plans.router, prefix="/ai", tags=["AI - Giáo Án"])

# Router cho đề thi
api_router.include_router(exams.router, prefix="/ai", tags=["AI - Đề Thi"]) # <<< THÊM DÒNG NÀY