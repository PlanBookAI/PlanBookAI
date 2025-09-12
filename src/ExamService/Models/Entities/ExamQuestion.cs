using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamService.Models.Entities
{
    [Table("exam_questions", Schema = "assessment")]
    public class ExamQuestion
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("exam_id")]
        public Guid DeThiId { get; set; }

        [Required]
        [Column("question_id")]
        public Guid CauHoiId { get; set; }

        [Required]
        [Column("question_order")]
        public int ThuTu { get; set; }

        [Required]
        [Column("points")]
        public decimal Diem { get; set; } = 1.0M;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("DeThiId")]
        public virtual DeThi DeThi { get; set; } = null!;

        [ForeignKey("CauHoiId")]
        public virtual CauHoi CauHoi { get; set; } = null!;
    }
}
