using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models.Entities
{
    [Table("chu_de", Schema = "content")]
    public class ChuDe
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        [Column("name")]
        public string Ten { get; set; } = string.Empty;

        [Column("description")]
        public string? MoTa { get; set; }

        // Phân cấp chủ đề
        [Column("parent_id")]
        public Guid? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public ChuDe? ChuDeCha { get; set; }
        public virtual ICollection<ChuDe> ChuDeCon { get; set; } = new List<ChuDe>();

        // Môn học và khối
        [StringLength(100)]
        [Column("subject")]
        public string MonHoc { get; set; } = "HOA_HOC";

        [Column("grade")]
        public int? Khoi { get; set; }

        // Audit
        [Column("created_by")]
        public Guid? TaoBoi { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;
    }
}