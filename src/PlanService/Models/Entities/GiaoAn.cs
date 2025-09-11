using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PlanService.Models.Entities
{
    [Table("lesson_plans", Schema = "content")]
    public class GiaoAn
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string TieuDe { get; set; } = string.Empty;

        [Column("objectives")]
        public string? MucTieu { get; set; }

        [Required]
        [Column("content")]
        public Dictionary<string, object> NoiDung { get; set; } = new();

        [Required]
        [StringLength(100)]
        [Column("subject")]
        public string MonHoc { get; set; } = "HOA_HOC";

        [Required]
        [Column("grade")]
        public int Lop { get; set; }

        [Required]
        [Column("teacher_id")]
        public Guid GiaoVienId { get; set; }

        [Column("template_id")]
        public Guid? MauGiaoAnId { get; set; }

        [Column("status")]
        public string TrangThai { get; set; } = "DRAFT";

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        [Column("topic_id")]
        public Guid? ChuDeId { get; set; }

        // Navigation properties 
        [ForeignKey("MauGiaoAnId")]
        public virtual MauGiaoAn? MauGiaoAn { get; set; }

        [ForeignKey("ChuDeId")]
        public virtual ChuDe? ChuDe { get; set; }
    }
}
