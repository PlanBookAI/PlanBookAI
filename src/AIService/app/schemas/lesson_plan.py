from pydantic import BaseModel, ConfigDict
from datetime import datetime

class LessonPlanBase(BaseModel):
    source_type: str
    prompt: str | None = None
    content: str

class LessonPlanCreate(LessonPlanBase):
    pass

class LessonPlanOut(LessonPlanBase):
    id: int
    created_at: datetime

    model_config = ConfigDict(from_attributes=True)