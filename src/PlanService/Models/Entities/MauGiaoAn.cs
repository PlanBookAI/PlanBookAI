using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace PlanService.Models.Entities
{
    [Table("lesson_templates", Schema = "content")]
    public class MauGiaoAn
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        [Column("title")]
        public string TieuDe { get; set; } = string.Empty;

        [Column("description")]
        public string? MoTa { get; set; }

        [Required]
        [Column("template_content", TypeName = "jsonb")]
        public Dictionary<string, object> NoiDungMau { get; set; } = new Dictionary<string, object>();

        [Required]
        [StringLength(100)] // DB2: subject varchar(100)
        [Column("subject")]
        public string MonHoc { get; set; } = string.Empty;

        [Column("grade")] // DB2: grade integer (nullable)
        public int? Lop { get; set; }

        [Required]
        [Column("created_by")]
        public Guid NguoiTaoId { get; set; }

        [Required]
        [StringLength(50)] // DB2: status varchar(50)
        [Column("status")]
        public string TrangThai { get; set; } = "ACTIVE";

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;
    }
}
