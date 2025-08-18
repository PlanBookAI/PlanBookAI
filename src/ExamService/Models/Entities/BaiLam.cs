using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskService.Models.Entities
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

        [Required]
        [Column("answers")]
        public string DapAn { get; set; } = string.Empty;

        [Column("submitted_at")]
        public DateTime NopLuc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("HocSinhId")]
        public virtual HocSinh HocSinh { get; set; } = null!;

        [ForeignKey("DeThiId")]
        public virtual DeThi DeThi { get; set; } = null!;

        public virtual KetQua KetQua { get; set; } = null!;
    }
}