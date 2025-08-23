using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO cơ sở chứa các thông tin chung khi tạo đề thi.
    /// </summary>
    public class TaoDeThiBaseDTO
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5 đến 250 ký tự.")]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Môn học không được để trống.")]
        public string MonHoc { get; set; } = string.Empty;

        [Range(10, 12, ErrorMessage = "Khối lớp phải từ 10 đến 12.")]
        public int KhoiLop { get; set; }

        [Range(15, 180, ErrorMessage = "Thời gian làm bài phải từ 15 đến 180 phút.")]
        public int ThoiGianLamBai { get; set; }
    }

    /// <summary>
    /// DTO cho yêu cầu tạo đề thi tự động từ ma trận kiến thức.
    /// </summary>
    public class TaoDeThiTuDongDTO : TaoDeThiBaseDTO
    {
        [Required]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một yêu cầu về câu hỏi.")]
        public List<YeuCauCauHoiDTO> MaTranCauHoi { get; set; } = new();
    }

    public class YeuCauCauHoiDTO
    {
        [Required]
        public string ChuDe { get; set; } = string.Empty;
        [Required]
        public string DoKho { get; set; } = string.Empty;
        [Range(1, 50, ErrorMessage = "Số lượng câu hỏi phải từ 1 đến 50.")]
        public int SoLuong { get; set; }
    }

    /// <summary>
    /// DTO cho yêu cầu tạo đề thi từ một danh sách ID câu hỏi cụ thể.
    /// </summary>
    public class TaoDeThiTuNganHangDTO : TaoDeThiBaseDTO
    {
        [Required]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất một câu hỏi.")]
        public List<Guid> DanhSachCauHoiId { get; set; } = new();
    }

    /// <summary>
    /// DTO cho yêu cầu tạo đề thi ngẫu nhiên.
    /// </summary>
    public class TaoDeThiNgauNhienDTO : TaoDeThiBaseDTO
    {
        [Range(1, 100, ErrorMessage = "Số lượng câu hỏi phải từ 1 đến 100.")]
        public int SoLuongCauHoi { get; set; }
        public string? ChuDe { get; set; }
        public string? DoKho { get; set; }
    }

    /// <summary>
    /// DTO cho yêu cầu tạo đề thi từ một mẫu có sẵn.
    /// </summary>
    public class TaoDeThiTuMauDTO : TaoDeThiBaseDTO
    {
        [Required]
        public Guid MauDeThiId { get; set; }
    }
}