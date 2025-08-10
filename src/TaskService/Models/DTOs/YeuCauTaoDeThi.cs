using System.ComponentModel.DataAnnotations;

namespace TaskService.Models.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tạo đề thi ngẫu nhiên
    /// </summary>
    public class YeuCauTaoDeThi
    {
        [Required(ErrorMessage = "Tiêu đề đề thi là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public string MonHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Độ khó là bắt buộc")]
        public string DoKho { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số lượng câu hỏi là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Số lượng câu hỏi phải từ 1 đến 100")]
        public int SoLuongCauHoi { get; set; }

        [Required(ErrorMessage = "Thời gian làm bài là bắt buộc")]
        [Range(15, 180, ErrorMessage = "Thời gian làm bài phải từ 15 đến 180 phút")]
        public int ThoiGianLam { get; set; }

        // Constructor để đảm bảo object có thể được tạo
        public YeuCauTaoDeThi() { }

        // Constructor với parameters
        public YeuCauTaoDeThi(string tieuDe, string monHoc, string doKho, int soLuongCauHoi, int thoiGianLam)
        {
            TieuDe = tieuDe;
            MonHoc = monHoc;
            DoKho = doKho;
            SoLuongCauHoi = soLuongCauHoi;
            ThoiGianLam = thoiGianLam;
        }
    }
}
