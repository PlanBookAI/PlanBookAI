using ExamService.Models.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ExamService.Documents
{
    public class DeThiDocument : IDocument
    {
        private readonly DeThi _deThi;

        public DeThiDocument(DeThi deThi)
        {
            _deThi = deThi;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text(_deThi.TieuDe).Bold().FontSize(20);
                column.Item().Text($"Môn: {_deThi.MonHoc} - Khối: {_deThi.KhoiLop}");
                column.Item().Text($"Thời gian làm bài: {_deThi.ThoiGianLamBai} phút");
                column.Item().PaddingVertical(15).LineHorizontal(1);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                var sortedQuestions = _deThi.ExamQuestions.OrderBy(q => q.ThuTu).ToList();

                foreach (var eq in sortedQuestions)
                {
                    var cauHoi = eq.CauHoi;
                    column.Item().PaddingBottom(15).Text(text =>
                    {
                        text.Span($"Câu {eq.ThuTu}: ").Bold();
                        text.Span(cauHoi.NoiDung);
                    });

                    // Hiển thị các lựa chọn A, B, C, D
                    var sortedChoices = cauHoi.LuaChons.OrderBy(c => c.MaLuaChon).ToList();
                    column.Item().PaddingLeft(20).Grid(grid =>
                    {
                        grid.Columns(2); // Chia thành 2 cột
                        for (int i = 0; i < sortedChoices.Count; i++)
                        {
                            var choice = sortedChoices[i];
                            var choiceLetter = (char)('A' + i);
                            grid.Item().Text($"{choiceLetter}. {choice.NoiDung}");
                        }
                    });

                    column.Item().PaddingVertical(10);
                }
            });
        }
    }
}