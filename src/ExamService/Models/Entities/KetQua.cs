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

        [Column("answer_sheet_id")]
        public Guid BaiLamId { get; set; }

        [Column("score")]
        public decimal Diem { get; set; }

        [Column("max_score")]
        public decimal DiemToiDa { get; set; }

        [Column("percentage")]
        public decimal TyLe { get; set; }

        [Column("time_taken_minutes")]
        public int ThoiGianLam { get; set; }

        [Column("submitted_at")]
        public DateTime NopLuc { get; set; }

        [Column("graded_at")]
        public DateTime? ChamLuc { get; set; }

        [Column("grader_id")]
        public Guid? NguoiChamId { get; set; }

        [Column("feedback")]
        public string? NhanXet { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("HocSinhId")]
        public virtual HocSinh HocSinh { get; set; } = null!;

        [ForeignKey("DeThiId")]
        public virtual DeThi DeThi { get; set; } = null!;

        [ForeignKey("BaiLamId")]
        public virtual BaiLam BaiLam { get; set; } = null!;
    }
}