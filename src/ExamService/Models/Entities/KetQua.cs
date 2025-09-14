using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Models.Entities
{
    [Table("student_results", Schema = "students")]
    public class KetQua
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("student_id")]
        public Guid HocSinhId { get; set; }

        [Required]
        [Column("exam_id")]
        public Guid DeThiId { get; set; }

        [Column("score")]
        public decimal? Diem { get; set; }

        [Column("actual_duration")]
        public int? ThoiGianLam { get; set; }

        [Column("answer_details", TypeName = "jsonb")]
        public string? ChiTietDapAn { get; set; }

        [Column("grading_method")]
        public string? PhuongPhapCham { get; set; }

        [Column("notes")]
        public string? GhiChu { get; set; }

        [Column("exam_date")]
        public DateTime? NgayThi { get; set; }

        [Column("graded_at")]
        public DateTime ChamLuc { get; set; } = DateTime.UtcNow;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("HocSinhId")]
        public virtual HocSinh HocSinh { get; set; } = null!;

        [ForeignKey("DeThiId")]
        public virtual DeThi DeThi { get; set; } = null!;

    }
}