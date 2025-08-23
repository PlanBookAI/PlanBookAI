using ExamService.Models.Entities;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace ExamService.Helpers
{
    public static class WordExportHelper
    {
        public static byte[] CreateDeThiDocument(DeThi deThi)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Tạo một tài liệu Word mới trong memory stream
                using (var document = DocX.Create(memoryStream))
                {
                    // --- Header ---
                    var title = document.InsertParagraph(deThi.TieuDe);
                    title.Bold().FontSize(18).Alignment = Alignment.center;

                    document.InsertParagraph($"Môn: {deThi.MonHoc} - Khối: {deThi.KhoiLop}")
                        .FontSize(12).Alignment = Alignment.center;
                    document.InsertParagraph($"Thời gian làm bài: {deThi.ThoiGianLamBai} phút")
                        .FontSize(12).Alignment = Alignment.center;
                    document.InsertParagraph(); // Thêm một dòng trống

                    // --- Content ---
                    var sortedQuestions = deThi.ExamQuestions.OrderBy(q => q.ThuTu).ToList();

                    foreach (var eq in sortedQuestions)
                    {
                        var cauHoi = eq.CauHoi;

                        // Nội dung câu hỏi
                        var pQuestion = document.InsertParagraph();
                        pQuestion.Append($"Câu {eq.ThuTu}: ").Bold();
                        pQuestion.Append(cauHoi.NoiDung);

                        // Các lựa chọn
                        var sortedChoices = cauHoi.LuaChons.OrderBy(c => c.ThuTu).ToList();

                        // Tạo list có thứ tự A, B, C, D
                        var list = document.AddList(listType: ListItemType.Numbered, startNumber: 1);
                        for (int i = 0; i < sortedChoices.Count; i++)
                        {
                            var choice = sortedChoices[i];
                            var choiceLetter = (char)('A' + i);
                            // Thêm lựa chọn vào list
                            document.AddListItem(list, $"{choiceLetter}. {choice.NoiDung}");
                        }
                        document.InsertList(list);
                        document.InsertParagraph(); // Thêm dòng trống giữa các câu hỏi
                    }

                    document.Save();
                }
                return memoryStream.ToArray();
            }
        }
    }
}