using System.ComponentModel.DataAnnotations;

namespace UserService.Repositories
{
    public class HoSoNguoiDung
    {
        [Key]
        public Guid UserId { get; set; }
        
        public Guid NguoiDungId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string HoTen { get; set; } = string.Empty;
        
        [Phone]
        [StringLength(20)]
        public string? SoDienThoai { get; set; }
        
        public DateTime? NgaySinh { get; set; }
        
        [StringLength(500)]
        public string? DiaChi { get; set; }
        
        [Url]
        [StringLength(500)]
        public string? AnhDaiDienUrl { get; set; }
        
        [StringLength(1000)]
        public string? MoTaBanThan { get; set; }
        
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;
        
        // Navigation property to NguoiDung
        public virtual UserService.Models.NguoiDung? NguoiDung { get; set; }
    }
}
