using System.ComponentModel.DataAnnotations;

namespace PlanService.Models.DTOs
{
    /// <summary>
    /// DTO (Data Transfer Object) dùng để nhận dữ liệu từ client khi tạo một mẫu giáo án.
    /// </summary>
    public class YeuCauTaoMauGiaoAn
    {
        [Required(ErrorMessage = "Tiêu đề mẫu giáo án là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string TieuDe { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Nội dung mẫu là bắt buộc")]
        public object NoiDungMau { get; set; } = new object();

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Môn học không được vượt quá 100 ký tự")]
        public string MonHoc { get; set; } = "HOA_HOC";

        [Range(10, 12, ErrorMessage = "Khối phải từ 10-12")]
        public int? Khoi { get; set; }

        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string TrangThai { get; set; } = "ACTIVE";
    }
}