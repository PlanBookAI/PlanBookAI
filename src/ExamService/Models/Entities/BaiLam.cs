using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Models.Entities
{
    [Table("answer_sheets", Schema = "students")]
    public class BaiLam
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

        [Column("image_url")]
        public string? AnhBaiLam { get; set; }

        [Column("ocr_result", TypeName = "jsonb")]
        public string? KetQuaOCR { get; set; }

        [Column("ocr_status")]
        public string TrangThaiOCR { get; set; } = "PENDING";

        [Column("ocr_accuracy")]
        public decimal? DoChinhXacOCR { get; set; }

        [Column("processed_at")]
        public DateTime? XuLyLuc { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("HocSinhId")]
        public virtual HocSinh HocSinh { get; set; } = null!;

        [ForeignKey("DeThiId")]
        public virtual DeThi DeThi { get; set; } = null!;

        public virtual KetQua KetQua { get; set; } = null!;
    }
}