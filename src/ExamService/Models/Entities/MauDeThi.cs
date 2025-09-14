using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Models.Entities
{
    [Table("exam_templates", Schema = "assessment")]
    public class MauDeThi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("title")]
        public string TieuDe { get; set; } = string.Empty;

        [Column("description")]
        public string? MoTa { get; set; }

        [Required]
        [Column("subject")]
        public string MonHoc { get; set; } = "HOA_HOC";

        [Column("grade")]
        public int? KhoiLop { get; set; }

        [Column("duration_minutes")]
        public int? ThoiGianLam { get; set; }

        [Column("total_score")]
        public decimal? TongDiem { get; set; }

        /// <summary>
        /// Cấu trúc ma trận câu hỏi, lưu dưới dạng JSON.
        /// Ví dụ: {"easy": 5, "medium": 10, "hard": 3, "topics": ["topic1", "topic2"]}
        /// </summary>
        [Column("structure", TypeName = "jsonb")]
        public string? CauTruc { get; set; }

        [Required]
        [Column("created_by")]
        public Guid NguoiTaoId { get; set; }

        [Column("status")]
        public string TrangThai { get; set; } = "ACTIVE";

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;
    }
}