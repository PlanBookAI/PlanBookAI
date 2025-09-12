using ExamService.Models.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Linq;

namespace ExamService.Helpers
{
    public static class PdfExportHelper
    {
        public static byte[] CreateDeThiDocument(DeThi deThi)
        {
            // Thiết lập font tiếng Việt
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font titleFont = new Font(baseFont, 18, Font.BOLD);
            Font headerFont = new Font(baseFont, 12, Font.NORMAL);
            Font questionFont = new Font(baseFont, 11, Font.BOLD);
            Font bodyFont = new Font(baseFont, 11, Font.NORMAL);
            Font choiceFont = new Font(baseFont, 11, Font.NORMAL);

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // --- Header ---
                document.Add(new Paragraph(deThi.TieuDe, titleFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph($"Môn: {deThi.MonHoc} - Khối: {deThi.KhoiLop}", headerFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph($"Thời gian làm bài: {deThi.ThoiGianLamBai} phút", headerFont) { Alignment = Element.ALIGN_CENTER });
                document.Add(new Paragraph(" ")); // Dòng trống

                // --- Content ---
                var sortedQuestions = deThi.ExamQuestions.OrderBy(q => q.ThuTu).ToList();
                foreach (var eq in sortedQuestions)
                {
                    var cauHoi = eq.CauHoi;

                    // Nội dung câu hỏi
                    var questionParagraph = new Paragraph();
                    var questionChunk = new Chunk($"Câu {eq.ThuTu}: ", questionFont);
                    var contentChunk = new Chunk(cauHoi.NoiDung, bodyFont);
                    questionParagraph.Add(questionChunk);
                    questionParagraph.Add(contentChunk);
                    document.Add(questionParagraph);

                    // Các lựa chọn
                    var sortedChoices = cauHoi.LuaChons.OrderBy(c => c.ThuTu).ToList();
                    for (int i = 0; i < sortedChoices.Count; i++)
                    {
                        var choice = sortedChoices[i];
                        var choiceLetter = (char)('A' + i);

                        // Tạo một Paragraph cho mỗi lựa chọn với thụt lề
                        var choiceParagraph = new Paragraph($"{choiceLetter}. {choice.NoiDung}", choiceFont)
                        {
                            IndentationLeft = 20f // Thụt lề đầu dòng
                        };
                        document.Add(choiceParagraph);
                    }
                    document.Add(new Paragraph(" ")); // Thêm dòng trống giữa các câu hỏi
                }

                document.Close();
                return memoryStream.ToArray();
            }
        }
    }
}