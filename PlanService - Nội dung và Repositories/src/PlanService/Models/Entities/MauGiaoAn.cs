using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models.Entities
{
    /// <summary>
    /// Lớp entity đại diện cho một Mẫu Giáo Án.
    /// </summary>
    public class MauGiaoAn
    {
        [Key] // Đánh dấu đây là khóa chính.
        public Guid Id { get; set; }

        [Required] // Trường này là bắt buộc.
        public string TenMau { get; set; }
        
        public string NoiDungTomTat { get; set; }
        
        public string GhiChu { get; set; }
        
        // Mặc định lưu thời điểm tạo mẫu giáo án.
        public DateTime NgayTao { get; set; }
    }
}