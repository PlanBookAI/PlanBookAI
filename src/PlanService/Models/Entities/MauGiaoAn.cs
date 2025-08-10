using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column("template_content")]
        public string NoiDungMau { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("subject")]
        public string MonHoc { get; set; } = string.Empty;

        [Required]
        [Column("grade")]
        public int Lop { get; set; }

        [Column("chu_de_id")]
        public Guid? ChuDeId { get; set; }

        [Required]
        [Column("created_by")]
        public Guid NguoiTaoId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string TrangThai { get; set; } = "ACTIVE";

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        [ForeignKey("ChuDeId")]
        public virtual ChuDe? ChuDe { get; set; }

        public virtual ICollection<GiaoAn> DanhSachGiaoAn { get; set; } = new List<GiaoAn>();
    }
}