using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ExamService.Models.Entities;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace ExamService.Helpers
{
    public static class WordExportHelper
    {
        public static byte[] CreateDeThiDocument(DeThi deThi)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                    {
                        // Thiết lập thông tin tài liệu
                        var docProps = wordDocument.AddCoreFilePropertiesPart();
                        
                        // Tạo XML trực tiếp để thiết lập thuộc tính tài liệu
                        using (var writer = XmlWriter.Create(docProps.GetStream()))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement("cp", "coreProperties", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
                            writer.WriteAttributeString("xmlns", "dc", null, "http://purl.org/dc/elements/1.1/");
                            writer.WriteAttributeString("xmlns", "dcterms", null, "http://purl.org/dc/terms/");
                            
                            // Tiêu đề
                            writer.WriteStartElement("dc", "title", null);
                            writer.WriteString(deThi.TieuDe);
                            writer.WriteEndElement();
                            
                            // Chủ đề
                            writer.WriteStartElement("dc", "subject", null);
                            writer.WriteString($"Đề thi {deThi.MonHoc}");
                            writer.WriteEndElement();
                            
                            // Tác giả
                            writer.WriteStartElement("dc", "creator", null);
                            writer.WriteString("PlanBookAI ExamService");
                            writer.WriteEndElement();
                            
                            // Thời gian tạo
                            writer.WriteStartElement("dcterms", "created", null);
                            writer.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", "dcterms:W3CDTF");
                            writer.WriteString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                            writer.WriteEndElement();
                            
                            writer.WriteEndElement(); // coreProperties
                            writer.WriteEndDocument();
                        }
                        
                        // Thiết lập style
                        var stylesPart = wordDocument.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
                        stylesPart.Styles = new Styles();
                        
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());

                        // --- Header ---
                        AddParagraph(body, deThi.TieuDe, new RunProperties(new Bold(), new FontSize() { Val = "36" }), JustificationValues.Center);
                        AddParagraph(body, $"Môn: {deThi.MonHoc} - Khối: {deThi.KhoiLop}", new RunProperties(new FontSize() { Val = "24" }), JustificationValues.Center);
                        AddParagraph(body, $"Thời gian làm bài: {deThi.ThoiGianLamBai} phút", new RunProperties(new FontSize() { Val = "24" }), JustificationValues.Center);
                        
                        // Thêm hướng dẫn nếu có
                        if (!string.IsNullOrEmpty(deThi.MoTa))
                        {
                            AddParagraph(body, "Hướng dẫn:", new RunProperties(new Bold(), new FontSize() { Val = "24" }));
                            AddParagraph(body, deThi.MoTa);
                        }
                        
                        AddParagraph(body, ""); // Dòng trống
                        
                        // Thêm đường kẻ ngang
                        Paragraph line = body.AppendChild(new Paragraph());
                        Run lineRun = line.AppendChild(new Run());
                        lineRun.AppendChild(new Break() { Type = BreakValues.TextWrapping });
                        ParagraphProperties pPr = line.AppendChild(new ParagraphProperties());
                        pPr.AppendChild(new ParagraphBorders(
                            new TopBorder() { Val = BorderValues.Single, Size = 24 }
                        ));

                        // --- Content ---
                        var sortedQuestions = deThi.ExamQuestions.OrderBy(q => q.ThuTu).ToList();
                        foreach (var eq in sortedQuestions)
                        {
                            var cauHoi = eq.CauHoi;

                            // Nội dung câu hỏi
                            Paragraph pQuestion = body.AppendChild(new Paragraph());
                            Run rQuestion = pQuestion.AppendChild(new Run());
                            rQuestion.AppendChild(new Text($"Câu {eq.ThuTu} ({eq.Diem} điểm): ") { Space = SpaceProcessingModeValues.Preserve });
                            rQuestion.RunProperties = new RunProperties(new Bold());
                            pQuestion.AppendChild(new Run(new Text(cauHoi.NoiDung)));

                            // Các lựa chọn
                            if (cauHoi.LoaiCauHoi == "MULTIPLE_CHOICE")
                            {
                                var sortedChoices = cauHoi.LuaChons.OrderBy(c => c.MaLuaChon).ToList();
                                for (int i = 0; i < sortedChoices.Count; i++)
                                {
                                    var choice = sortedChoices[i];
                                    AddParagraph(body, $"{choice.MaLuaChon}. {choice.NoiDung}", null, null, "200");
                                }
                            }
                            else if (cauHoi.LoaiCauHoi == "ESSAY" || cauHoi.LoaiCauHoi == "SHORT_ANSWER")
                            {
                                // Thêm khoảng trống để học sinh viết câu trả lời
                                AddParagraph(body, "Trả lời:", null, null, "200");
                                
                                // Thêm bảng để học sinh viết câu trả lời
                                Table table = new Table();
                                TableProperties tblPr = new TableProperties(
                                    new TableBorders(
                                        new TopBorder() { Val = BorderValues.Single },
                                        new BottomBorder() { Val = BorderValues.Single },
                                        new LeftBorder() { Val = BorderValues.Single },
                                        new RightBorder() { Val = BorderValues.Single },
                                        new InsideHorizontalBorder() { Val = BorderValues.Single },
                                        new InsideVerticalBorder() { Val = BorderValues.Single }
                                    )
                                );
                                table.AppendChild(tblPr);
                                
                                // Thêm 5 dòng trống
                                for (int i = 0; i < 5; i++)
                                {
                                    TableRow tr = new TableRow();
                                    TableCell tc = new TableCell();
                                    tc.AppendChild(new Paragraph(new Run(new Text(""))));
                                    tr.AppendChild(tc);
                                    table.AppendChild(tr);
                                }
                                
                                body.AppendChild(table);
                            }
                            
                            AddParagraph(body, ""); // Dòng trống
                        }
                        
                        // Thêm footer
                        AddParagraph(body, "--- HẾT ---", new RunProperties(new Bold(), new FontSize() { Val = "24" }), JustificationValues.Center);
                        AddParagraph(body, $"Tổng số câu: {sortedQuestions.Count} - Tổng điểm: {deThi.DiemToiDa}", 
                            new RunProperties(new Italic(), new FontSize() { Val = "20" }), JustificationValues.Center);
                    }
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Tạo file Word đơn giản với thông báo lỗi
                using (var memoryStream = new MemoryStream())
                {
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());
                        
                        AddParagraph(body, "Đã xảy ra lỗi khi tạo file Word:", new RunProperties(new Bold(), new Color() { Val = "FF0000" }));
                        AddParagraph(body, ex.Message);
                    }
                    return memoryStream.ToArray();
                }
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