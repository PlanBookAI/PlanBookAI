#if false
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PlanService.Models.DTOs;
using PlanService.Repositories;

namespace PlanService.Services.Export
{
    /// <summary>
    /// Service xuất PDF cho giáo án:
    /// - A4, lề chuẩn, Times 12 (nếu khả dụng), header/footer + số trang
    /// - Bookmarks (Outlines) cho các mục lớn: Tiêu đề, Mục tiêu, Nội dung
    /// - Nội dung lấy từ entity giáo án (title, objectives, content json)
    /// </summary>
    public class XuatGiaoAnPdfService : IXuatGiaoAnPdfService
    {
        private readonly IGiaoAnRepository _repo;

        public XuatGiaoAnPdfService(IGiaoAnRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiPhanHoi<byte[]>> XuatPdfAsync(Guid giaoAnId, Guid teacherId)
        {
            try
            {
                var giaoAn = await _repo.GetByIdAndTeacherIdAsync(giaoAnId, teacherId);
                if (giaoAn == null) return ApiPhanHoi<byte[]>.ThatBai("Không tìm thấy giáo án");

                using var ms = new MemoryStream();
                using (var doc = new PdfDocument())
                {
                    // Trang A4
                    var page = doc.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    var gfx = XGraphics.FromPdfPage(page);

                    // Fonts
                    var fontTitle = new XFont("Times New Roman", 16, XFontStyle.Bold);
                    var fontHeading = new XFont("Times New Roman", 14, XFontStyle.Bold);
                    var fontBody = new XFont("Times New Roman", 12, XFontStyle.Regular);

                    // Lề (72 dpi ~ 1 inch; 2.54cm ~ 72pt)
                    double margin = 72;
                    double y = margin;

                    // Header
                    gfx.DrawString("Trường: ..................   Môn: Hóa học", fontBody, XBrushes.Black, new XRect(margin, y - 40, page.Width - 2 * margin, 20), XStringFormats.TopLeft);

                    // Tiêu đề
                    gfx.DrawString(giaoAn.TieuDe ?? "Giáo án", fontTitle, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, 24), XStringFormats.TopLeft);
                    var outlineRoot = doc.Outlines.Add(giaoAn.TieuDe ?? "Giáo án", page, true);
                    y += 40;

                    // Mục tiêu
                    if (!string.IsNullOrWhiteSpace(giaoAn.MucTieu))
                    {
                        gfx.DrawString("Mục tiêu", fontHeading, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                        outlineRoot.Add("Mục tiêu", page, true);
                        y += 24;

                        y = DrawParagraph(gfx, giaoAn.MucTieu, fontBody, margin, y, page.Width - 2 * margin, page.Height - margin);
                        y += 16;
                    }

                    // Nội dung
                    if (!string.IsNullOrWhiteSpace(giaoAn.NoiDung))
                    {
                        gfx.DrawString("Nội dung", fontHeading, XBrushes.Black, new XRect(margin, y, page.Width - 2 * margin, 20), XStringFormats.TopLeft);
                        outlineRoot.Add("Nội dung", page, true);
                        y += 24;

                        try
                        {
                            var json = System.Text.Json.JsonDocument.Parse(giaoAn.NoiDung).RootElement;
                            foreach (var prop in json.EnumerateObject())
                            {
                                y = DrawParagraph(gfx, $"{prop.Name}: {prop.Value.ToString()}", fontBody, margin, y, page.Width - 2 * margin, page.Height - margin);
                                y += 8;
                            }
                        }
                        catch
                        {
                            y = DrawParagraph(gfx, giaoAn.NoiDung, fontBody, margin, y, page.Width - 2 * margin, page.Height - margin);
                        }
                    }

                    // Footer: số trang
                    for (int i = 0; i < doc.Pages.Count; i++)
                    {
                        var p = doc.Pages[i];
                        var g = XGraphics.FromPdfPage(p);
                        g.DrawString($"{i + 1}", fontBody, XBrushes.Black, new XRect(0, p.Height - margin + 20, p.Width, 20), XStringFormats.Center);
                    }

                    doc.Save(ms);
                }

                return ApiPhanHoi<byte[]>.ThanhCongOk(ms.ToArray(), "Xuất PDF thành công");
            }
            catch (Exception ex)
            {
                return ApiPhanHoi<byte[]>.ThatBai("Lỗi khi xuất PDF: " + ex.Message);
            }
        }

        private double DrawParagraph(XGraphics gfx, string text, XFont font, double x, double y, double width, double maxY)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var line = "";
            foreach (var w in words)
            {
                var test = string.IsNullOrEmpty(line) ? w : line + " " + w;
                var size = gfx.MeasureString(test, font);
                if (size.Width > width)
                {
                    gfx.DrawString(line, font, XBrushes.Black, new XRect(x, y, width, font.Height), XStringFormats.TopLeft);
                    y += font.Height + 2;
                    line = w;
                    if (y > maxY) y = maxY; // giữ đơn giản, không phân trang trong bước này
                }
                else
                {
                    line = test;
                }
            }
            if (!string.IsNullOrEmpty(line))
            {
                gfx.DrawString(line, font, XBrushes.Black, new XRect(x, y, width, font.Height), XStringFormats.TopLeft);
                y += font.Height + 2;
            }
            return y;
        }
    }
}
#endif