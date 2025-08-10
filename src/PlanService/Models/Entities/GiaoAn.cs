using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("subject")]
        public string MonHoc { get; set; } = string.Empty;

        [Required]
        [Column("grade")]
        public int Lop { get; set; }

        [Required]
        [Column("teacher_id")]
        public Guid GiaoVienId { get; set; }

        [Column("template_id")]
        public Guid? MauGiaoAnId { get; set; }

        [Column("chu_de_id")]
        public Guid? ChuDeId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string TrangThai { get; set; } = "DRAFT";

        [Column("duration_minutes")]
        public int? ThoiLuongPhut { get; set; }

        [Column("class_session")]
        public string? BuoiHoc { get; set; }

        [Column("teaching_date")]
        public DateTime? NgayGiangDay { get; set; }

        [Column("notes")]
        public string? GhiChu { get; set; }

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        [ForeignKey("MauGiaoAnId")]
        public virtual MauGiaoAn? MauGiaoAn { get; set; }

        [ForeignKey("ChuDeId")]
        public virtual ChuDe? ChuDe { get; set; }

        // Navigation properties cho mối quan hệ 1-N
        public virtual ICollection<NoiDungGiaoDuc> NoiDungGiaoDucs { get; set; } = new List<NoiDungGiaoDuc>();
        public virtual ICollection<MucTieu> MucTieus { get; set; } = new List<MucTieu>();
    }
}