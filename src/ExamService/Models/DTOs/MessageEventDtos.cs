using System;

namespace ExamService.Models.DTOs
{
    public class NewExamEventDTO
    {
        public Guid DeThiId { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public Guid NguoiTaoId { get; set; }
    }

    public class NewQuestionEventDTO
    {
        public Guid CauHoiId { get; set; }
        public Guid NguoiTaoId { get; set; }
        public string MonHoc { get; set; } = string.Empty;
    }

    public class ExamPublishedEventDTO
    {
        public Guid DeThiId { get; set; }
        public Guid NguoiTaoId { get; set; }
    }
}