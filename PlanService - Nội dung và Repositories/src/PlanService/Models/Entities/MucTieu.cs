using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models.Entities
{
    /// <summary>
    /// Lớp entity đại diện cho mục tiêu của một giáo án.
    /// </summary>
    public class MucTieu
    {
        [Key]
        public Guid Id { get; set; }

        public Guid GiaoAnId { get; set; } // Khóa ngoại để liên kết với Giáo Án.

        [ForeignKey("GiaoAnId")]
        public GiaoAn GiaoAn { get; set; } // Thuộc tính navigation để truy cập đối tượng Giáo Án.

        [Required]
        public string TieuDe { get; set; }

        public string MoTa { get; set; }

        [Required]
        public bool DatDuoc { get; set; } 
    }
}