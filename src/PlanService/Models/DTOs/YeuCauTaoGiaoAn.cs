using System.ComponentModel.DataAnnotations;
using PlanService.Models.Enums;

namespace PlanService.Models.DTOs
{
    /// <summary>
    /// DTO cho request tạo giáo án từ AI hoặc template
    /// </summary>
    public class YeuCauTaoGiaoAn
    {
        [Required(ErrorMessage = "Tiêu đề giáo án là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string TieuDe { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mục tiêu không được vượt quá 1000 ký tự")]
        public string? MucTieu { get; set; }

        [Required(ErrorMessage = "Chủ đề ID là bắt buộc")]
        public Guid ChuDeId { get; set; }

        public Guid? MauGiaoAnId { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public MonHoc MonHoc { get; set; } = MonHoc.HoaHoc;

        [Range(1, 5, ErrorMessage = "Thời lượng phải từ 1 đến 5 tiết")]
        public int ThoiLuongTiet { get; set; } = 1;

        [StringLength(100, ErrorMessage = "Lớp học không được vượt quá 100 ký tự")]
        public string? LopHoc { get; set; }

        [StringLength(2000, ErrorMessage = "Ghi chú không được vượt quá 2000 ký tự")]
        public string? GhiChu { get; set; }

        public bool SuDungAI { get; set; } = true;

        [StringLength(500, ErrorMessage = "Yêu cầu đặc biệt không được vượt quá 500 ký tự")]
        public string? YeuCauDacBiet { get; set; }
    }
}