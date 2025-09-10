#if false
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PlanService.Models.DTOs;
using PlanService.Repositories;

namespace PlanService.Services.Export
{
    /// <summary>
    /// Service xuất Word (.docx) cho giáo án:
    /// - Times New Roman 12pt, A4, lề chuẩn
    /// - Header/Footer + số trang
    /// - Mục lục (TOC) tự cập nhật khi mở
    /// - Nội dung từ title, objectives, content (json)
    /// </summary>
    public class XuatGiaoAnWordService : IXuatGiaoAnWordService
    {
        private readonly IGiaoAnRepository _repo;

        public XuatGiaoAnWordService(IGiaoAnRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiPhanHoi<byte[]>> XuatWordAsync(Guid giaoAnId, Guid teacherId)
        {
            try
            {
                var giaoAn = await _repo.GetByIdAndTeacherIdAsync(giaoAnId, teacherId);
                if (giaoAn == null) return ApiPhanHoi<byte[]>.ThatBai("Không tìm thấy giáo án");

                using var ms = new MemoryStream();
                using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    var mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body());

                    // Styles: Times New Roman 12pt + Heading1
                    var stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
                    stylePart.Styles = new Styles(
                        new DocDefaults(
                            new RunPropertiesDefault(
                                new RunProperties(
                                    new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                    new FontSize { Val = "24" } // 12pt
                                )
                            )
                        ),
                        new Style
                        {
                            Type = StyleValues.Paragraph,
                            StyleId = "Heading1",
                            StyleName = new StyleName { Val = "heading 1" },
                            BasedOn = new BasedOn { Val = "Normal" },
                            UIPriority = new UIPriority { Val = 9 },
                            PrimaryStyle = new PrimaryStyle(),
                            StyleParagraphProperties = new StyleParagraphProperties(new OutlineLevel { Val = 0 }),
                            StyleRunProperties = new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Bold(),
                                new FontSize { Val = "28" } // 14pt
                            )
                        }
                    );

                    // Header/Footer + số trang + A4 + lề
                    var sectProps = new SectionProperties();
                    var headerPart = mainPart.AddNewPart<HeaderPart>();
                    var headerRelId = mainPart.GetIdOfPart(headerPart);
                    headerPart.Header = new Header(new Paragraph(new Run(new Text("Trường: ..................   Môn: Hóa học"))));
                    var footerPart = mainPart.AddNewPart<FooterPart>();
                    var footerRelId = mainPart.GetIdOfPart(footerPart);
                    footerPart.Footer = new Footer(
                        new Paragraph(
                            new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                            new Run(new FieldChar { FieldCharType = FieldCharValues.Begin }),
                            new Run(new FieldCode(" PAGE ") { Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve }),
                            new Run(new FieldChar { FieldCharType = FieldCharValues.End })
                        )
                    );
                    sectProps.Append(
                        new HeaderReference { Type = HeaderFooterValues.Default, Id = headerRelId },
                        new FooterReference { Type = HeaderFooterValues.Default, Id = footerRelId },
                        new PageSize { Code = (UInt16Value)9U },
                        new PageMargin { Top = 1440, Bottom = 1440, Left = 1440, Right = 1440 }
                    );

                    var body = mainPart.Document.Body ??= new Body();

                    // Tiêu đề (Heading 1)
                    body.Append(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" }),
                            new Run(new Text(giaoAn.TieuDe ?? "Giáo án"))
                        )
                    );

                    // Mục lục (TOC)
                    body.Append(new Paragraph(new Run(new Text("Mục lục"))));
                    body.Append(
                        new Paragraph(
                            new Run(new FieldChar { FieldCharType = FieldCharValues.Begin }),
                            new Run(new FieldCode(" TOC \\o \"1-3\" \\h \\z \\u ") { Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve }),
                            new Run(new FieldChar { FieldCharType = FieldCharValues.Separate }),
                            new Run(new Text("")),
                            new Run(new FieldChar { FieldCharType = FieldCharValues.End })
                        )
                    );

                    // Mục tiêu
                    if (!string.IsNullOrWhiteSpace(giaoAn.MucTieu))
                    {
                        body.Append(
                            new Paragraph(
                                new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" }),
                                new Run(new Text("Mục tiêu"))
                            )
                        );
                        body.Append(new Paragraph(new Run(new Text(giaoAn.MucTieu))));
                    }

                    // Nội dung từ JSON
                    if (!string.IsNullOrWhiteSpace(giaoAn.NoiDung))
                    {
                        body.Append(
                            new Paragraph(
                                new ParagraphProperties(new ParagraphStyleId { Val = "Heading1" }),
                                new Run(new Text("Nội dung"))
                            )
                        );
                        try
                        {
                            var json = System.Text.Json.JsonDocument.Parse(giaoAn.NoiDung).RootElement;
                            foreach (var prop in json.EnumerateObject())
                            {
                                body.Append(new Paragraph(new Run(new Text($"{prop.Name}: {prop.Value.ToString()}"))));
                            }
                        }
                        catch
                        {
                            body.Append(new Paragraph(new Run(new Text(giaoAn.NoiDung))));
                        }
                    }

                    body.Append(sectProps);
                    mainPart.Document.Save();
                }

                return ApiPhanHoi<byte[]>.ThanhCongOk(ms.ToArray(), "Xuất Word thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<byte[]>.ThatBai("Lỗi khi xuất Word: " + ex.Message);
            }
        }
    }
}
#endif