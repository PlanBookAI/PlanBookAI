using System;

namespace PlanService.Models.DTOs
{
    /// <summary>
    /// DTO dùng để trả về thông tin của một mẫu giáo án cho client.
    /// </summary>
    public class PhanHoiMauGiaoAn
    {
        public Guid Id { get; set; }
        public string TenMauGiaoAn { get; set; } = string.Empty;
        public string NoiDungTomTat { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
    }
}