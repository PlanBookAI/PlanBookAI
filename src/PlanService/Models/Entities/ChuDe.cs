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
        [Column("ten")]
        public string Ten { get; set; } = string.Empty;

        [Column("mo_ta")]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(50)]
        [Column("mon_hoc")]
        public string MonHoc { get; set; } = string.Empty;

        [Column("thu_tu")]
        public int ThuTu { get; set; } = 0;

        [Column("is_active")]
        public bool LaHoatDong { get; set; } = true;

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        public virtual ICollection<GiaoAn> DanhSachGiaoAn { get; set; } = new List<GiaoAn>();
        public virtual ICollection<MauGiaoAn> DanhSachMauGiaoAn { get; set; } = new List<MauGiaoAn>();
    }
}