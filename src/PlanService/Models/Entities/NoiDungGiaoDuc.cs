using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlanService.Models.Enums;

namespace PlanService.Models.Entities
{
    /// <summary>
    /// Lớp entity đại diện cho nội dung giáo dục trong một giáo án.
    /// </summary>
    public class NoiDungGiaoDuc
    {
        [Key] // Đánh dấu đây là khóa chính của bảng.
        public Guid Id { get; set; }

        public Guid GiaoAnId { get; set; } // Khóa ngoại để liên kết với Giáo Án.

        [ForeignKey("GiaoAnId")] // Đánh dấu GiaoAn là đối tượng liên kết với khóa ngoại GiaoAnId.
        public GiaoAn GiaoAn { get; set; } // Thuộc tính navigation để truy cập đối tượng Giáo Án.

        [Required] 
        public string TieuDe { get; set; }

        public string MoTa { get; set; }

        public string NoiDungChiTiet { get; set; }

        public LoaiNoiDung LoaiNoiDung { get; set; } // Sử dụng enum để giới hạn loại nội dung.
    }
}