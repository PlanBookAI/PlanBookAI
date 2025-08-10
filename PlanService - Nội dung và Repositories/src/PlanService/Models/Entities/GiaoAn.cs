using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanService.Models.Entities
{
    /// <summary>
    /// Lớp entity đại diện cho một Giáo Án. Đây là lớp cha của NoiDungGiaoDuc và MucTieu.
    /// </summary>
    public class GiaoAn
    {
        [Key] // Đánh dấu đây là khóa chính.
        public Guid Id { get; set; }

        [Required]
        public string TenGiaoAn { get; set; }

        public string MoTa { get; set; }

        // Thuộc tính navigation để thiết lập mối quan hệ 1:N với NoiDungGiaoDuc.
        // Một Giáo Án có thể có nhiều NoiDungGiaoDuc.
        public ICollection<NoiDungGiaoDuc> NoiDungGiaoDucs { get; set; }

        // Thuộc tính navigation để thiết lập mối quan hệ 1:N với MucTieu.
        // Một Giáo Án có thể có nhiều Mục Tiêu.
        public ICollection<MucTieu> MucTieus { get; set; }
    }
}