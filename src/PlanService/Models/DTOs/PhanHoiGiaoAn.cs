using PlanService.Models.Enums;
using System.Collections.Generic;

namespace PlanService.Models.DTOs
{
    /// <summary>
    /// DTO cho response khi tạo hoặc lấy thông tin giáo án
    /// </summary>
    public class PhanHoiGiaoAn
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public Guid? GiaoAnId { get; set; }
        public ThongTinGiaoAn? GiaoAn { get; set; }
        public List<string> LoiValidation { get; set; } = new List<string>();
    }

    /// <summary>
    /// Thông tin chi tiết của giáo án
    /// </summary>
    public class ThongTinGiaoAn
    {
        public Guid Id { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string? MucTieu { get; set; }
        public Dictionary<string, object> NoiDung { get; set; } = new Dictionary<string, object>();
        public string MonHoc { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public int ThoiLuongTiet { get; set; }
        public string? LopHoc { get; set; }
        public string? GhiChu { get; set; }
        public string TenChuDe { get; set; } = string.Empty;

        // Thông tin template (nếu có)
        public Guid? MauGiaoAnId { get; set; }
        public string? TenMauGiaoAn { get; set; }

        // Metadata
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public Guid NguoiTaoId { get; set; }
        public string? TenNguoiTao { get; set; }

        // AI generation info
        public bool DuocTaoTuAI { get; set; }
        public string? YeuCauDacBiet { get; set; }
    }
}
