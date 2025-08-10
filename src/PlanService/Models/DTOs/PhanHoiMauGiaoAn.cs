using System;

namespace PlanService.Models.DTOs
{
    /// <summary>
    /// DTO dùng để trả về thông tin của một mẫu giáo án cho client.
    /// </summary>
    public class PhanHoiMauGiaoAn
    {
        public Guid Id { get; set; }
        public string TenMauGiaoAn { get; set; }
        public string NoiDungTomTat { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
    }
}