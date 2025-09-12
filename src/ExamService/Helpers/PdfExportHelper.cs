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
            try
            {
                // Thiết lập font tiếng Việt
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                // Nếu không tìm thấy font, sử dụng font mặc định
                if (!File.Exists(fontPath))
                {
                    fontPath = BaseFont.HELVETICA;
                }
                
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font titleFont = new Font(baseFont, 18, Font.BOLD);
                Font headerFont = new Font(baseFont, 12, Font.NORMAL);
                Font questionFont = new Font(baseFont, 11, Font.BOLD);
                Font bodyFont = new Font(baseFont, 11, Font.NORMAL);
                Font choiceFont = new Font(baseFont, 11, Font.NORMAL);
                Font footerFont = new Font(baseFont, 8, Font.ITALIC);

                using (var memoryStream = new MemoryStream())
                {
                    var document = new Document(PageSize.A4, 50, 50, 25, 25);
                    var writer = PdfWriter.GetInstance(document, memoryStream);
                    
                    // Thêm metadata
                    document.AddTitle(deThi.TieuDe);
                    document.AddSubject($"Đề thi {deThi.MonHoc}");
                    document.AddKeywords($"đề thi, {deThi.MonHoc}, khối {deThi.KhoiLop}");
                    document.AddCreator("PlanBookAI ExamService");
                    
                    document.Open();

                    // --- Header ---
                    document.Add(new Paragraph(deThi.TieuDe, titleFont) { Alignment = Element.ALIGN_CENTER });
                    document.Add(new Paragraph($"Môn: {deThi.MonHoc} - Khối: {deThi.KhoiLop}", headerFont) { Alignment = Element.ALIGN_CENTER });
                    document.Add(new Paragraph($"Thời gian làm bài: {deThi.ThoiGianLamBai} phút", headerFont) { Alignment = Element.ALIGN_CENTER });
                    
                    // Thêm hướng dẫn nếu có
                    if (!string.IsNullOrEmpty(deThi.MoTa))
                    {
                        document.Add(new Paragraph("Hướng dẫn:", headerFont));
                        document.Add(new Paragraph(deThi.MoTa, bodyFont));
                    }
                    
                    document.Add(new Paragraph(" ")); // Dòng trống
                    
                    // Vẽ đường kẻ phân cách
                    LineSeparator line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -5f);
                    document.Add(line);
                    document.Add(new Paragraph(" ")); // Dòng trống

                    // --- Content ---
                    var sortedQuestions = deThi.ExamQuestions.OrderBy(q => q.ThuTu).ToList();
                    foreach (var eq in sortedQuestions)
                    {
                        var cauHoi = eq.CauHoi;

                        // Nội dung câu hỏi
                        var questionParagraph = new Paragraph();
                        var questionChunk = new Chunk($"Câu {eq.ThuTu} ({eq.Diem} điểm): ", questionFont);
                        var contentChunk = new Chunk(cauHoi.NoiDung, bodyFont);
                        questionParagraph.Add(questionChunk);
                        questionParagraph.Add(contentChunk);
                        document.Add(questionParagraph);

                        // Các lựa chọn
                        if (cauHoi.LoaiCauHoi == "MULTIPLE_CHOICE")
                        {
                            var sortedChoices = cauHoi.LuaChons.OrderBy(c => c.MaLuaChon).ToList();
                            for (int i = 0; i < sortedChoices.Count; i++)
                            {
                                var choice = sortedChoices[i];
                                
                                // Tạo một Paragraph cho mỗi lựa chọn với thụt lề
                                var choiceParagraph = new Paragraph($"{choice.MaLuaChon}. {choice.NoiDung}", choiceFont)
                                {
                                    IndentationLeft = 20f // Thụt lề đầu dòng
                                };
                                document.Add(choiceParagraph);
                            }
                        }
                        else if (cauHoi.LoaiCauHoi == "ESSAY" || cauHoi.LoaiCauHoi == "SHORT_ANSWER")
                        {
                            // Thêm khoảng trống để học sinh viết câu trả lời
                            document.Add(new Paragraph("Trả lời:", bodyFont) { IndentationLeft = 20f });
                            
                            // Thêm đường kẻ để học sinh viết câu trả lời
                            for (int i = 0; i < 5; i++)
                            {
                                document.Add(new Paragraph("_".PadRight(80, '_'), bodyFont) { IndentationLeft = 20f });
                            }
                        }
                        
                        document.Add(new Paragraph(" ")); // Thêm dòng trống giữa các câu hỏi
                    }
                    
                    // Thêm footer
                    document.Add(new Paragraph("--- HẾT ---", headerFont) { Alignment = Element.ALIGN_CENTER });
                    document.Add(new Paragraph($"Tổng số câu: {sortedQuestions.Count} - Tổng điểm: {deThi.DiemToiDa}", footerFont) { Alignment = Element.ALIGN_CENTER });

                    document.Close();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về PDF trống với thông báo lỗi
                using (var memoryStream = new MemoryStream())
                {
                    var document = new Document(PageSize.A4);
                    PdfWriter.GetInstance(document, memoryStream);
                    document.Open();
                    document.Add(new Paragraph("Đã xảy ra lỗi khi tạo file PDF:"));
                    document.Add(new Paragraph(ex.Message));
                    document.Close();
                    return memoryStream.ToArray();
                }
            }
        }
    }
}