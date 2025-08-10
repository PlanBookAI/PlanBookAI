using System.ComponentModel.DataAnnotations;

namespace PlanService.Models.DTOs
{
    /// <summary>
    /// DTO (Data Transfer Object) dùng để nhận dữ liệu từ client khi tạo một mẫu giáo án.
    /// </summary>
    public class YeuCauTaoMauGiaoAn
    {
        [Required(ErrorMessage = "Tên mẫu giáo án là bắt buộc.")]
        public string TenMauGiaoAn { get; set; }

        [Required(ErrorMessage = "Nội dung tóm tắt là bắt buộc.")]
        public string NoiDungTomTat { get; set; }

        public string GhiChu { get; set; }
    }
}