using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Models.Entities
{
    // TODO: MauDeThi entity không khớp với database schema hiện tại
    // [Table("exam_templates", Schema = "assessment")]
    public class MauDeThi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("title")]
        public string TieuDe { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required]
        [Column("subject")]
        public string MonHoc { get; set; } = string.Empty;

        [Column("grade_level")]
        public int KhoiLop { get; set; }

        /// <summary>
        /// Cấu trúc ma trận câu hỏi, lưu dưới dạng JSON.
        /// Ví dụ: [{"ChuDe": "Axit", "DoKho": "De", "SoLuong": 10}, ...]
        /// </summary>
        [Required]
        [Column("structure", TypeName = "jsonb")]
        public string CauTruc { get; set; } = string.Empty;

        [Required]
        [Column("created_by")]
        public Guid NguoiTaoId { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;
    }
}