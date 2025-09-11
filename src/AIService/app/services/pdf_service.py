from fastapi import UploadFile
import pypdf
import io

async def extract_text_from_pdf(file: UploadFile) -> str:
    pdf_content = await file.read()
    pdf_reader = pypdf.PdfReader(io.BytesIO(pdf_content))
    text = ""
    for page in pdf_reader.pages:
        text += page.extract_text() or ""
    return text