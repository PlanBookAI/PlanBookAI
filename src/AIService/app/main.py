from fastapi import FastAPI
from app.api.v1.router import api_router

app = FastAPI(title="PlanBook AI Service")

app.include_router(api_router, prefix="/api/v1")

@app.get("/", tags=["Health Check"])
def read_root():
    return {"message": "AI Service is running!"}

#http://127.0.0.1:8000/docs