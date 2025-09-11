from sqlalchemy import Column, Integer, String, Text, DateTime, func
from app.db.base import Base

class LessonPlan(Base):
    __tablename__ = "lesson_plans"

    id = Column(Integer, primary_key=True, index=True)
    source_type = Column(String, nullable=False) # "pdf" or "prompt"
    prompt = Column(Text, nullable=True)
    content = Column(Text, nullable=False)
    created_at = Column(DateTime(timezone=True), server_default=func.now())