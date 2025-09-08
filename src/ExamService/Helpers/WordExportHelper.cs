using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ExamService.Models.Entities;
using System.IO;
using System.Linq;

namespace ExamService.Helpers
{
    public static class WordExportHelper
    {
        public static byte[] CreateDeThiDocument(DeThi deThi)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // --- Header ---
                    AddParagraph(body, deThi.TieuDe, new RunProperties(new Bold(), new FontSize() { Val = "36" }), JustificationValues.Center);
                    AddParagraph(body, $"Môn: {deThi.MonHoc} - Khối: {deThi.KhoiLop}", new RunProperties(new FontSize() { Val = "24" }), JustificationValues.Center);
                    AddParagraph(body, $"Thời gian làm bài: {deThi.ThoiGianLamBai} phút", new RunProperties(new FontSize() { Val = "24" }), JustificationValues.Center);
                    AddParagraph(body, ""); // Dòng trống

                    // --- Content ---
                    var sortedQuestions = deThi.ExamQuestions.OrderBy(q => q.ThuTu).ToList();
                    foreach (var eq in sortedQuestions)
                    {
                        var cauHoi = eq.CauHoi;

                        // Nội dung câu hỏi
                        Paragraph pQuestion = body.AppendChild(new Paragraph());
                        Run rQuestion = pQuestion.AppendChild(new Run());
                        rQuestion.AppendChild(new Text($"Câu {eq.ThuTu}: ") { Space = SpaceProcessingModeValues.Preserve });
                        rQuestion.RunProperties = new RunProperties(new Bold());
                        pQuestion.AppendChild(new Run(new Text(cauHoi.NoiDung)));

                        // Các lựa chọn
                        var sortedChoices = cauHoi.LuaChons.OrderBy(c => c.ThuTu).ToList();
                        for (int i = 0; i < sortedChoices.Count; i++)
                        {
                            var choice = sortedChoices[i];
                            var choiceLetter = (char)('A' + i);
                            AddParagraph(body, $"{choiceLetter}. {choice.NoiDung}", null, null, "200");
                        }
                        AddParagraph(body, ""); // Dòng trống
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private static void AddParagraph(Body body, string text, RunProperties? runProperties = null, JustificationValues? justification = null, string? leftIndent = null)
        {
            Paragraph para = body.AppendChild(new Paragraph());
            if (justification != null)
            {
                para.ParagraphProperties = new ParagraphProperties(new Justification() { Val = justification });
            }
            if (leftIndent != null)
            {
                para.ParagraphProperties = new ParagraphProperties(new Indentation() { Left = leftIndent });
            }

            Run run = para.AppendChild(new Run());
            if (runProperties != null)
            {
                run.RunProperties = (RunProperties)runProperties.CloneNode(true);
            }
            run.AppendChild(new Text(text));
        }
    }
}