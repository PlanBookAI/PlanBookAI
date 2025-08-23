using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tạo hoặc cập nhật một Đề thi.
    /// </summary>
    public class DeThiRequestDTO
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

        public string? HuongDan { get; set; }
    }

    /// <summary>
    /// DTO trả về thông tin chi tiết của một Đề thi.
    /// </summary>
    public class DeThiResponseDTO
    {
        public Guid Id { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string MonHoc { get; set; } = string.Empty;
        public int KhoiLop { get; set; }
        public int ThoiGianLamBai { get; set; }
        public string? HuongDan { get; set; }
        public string TrangThai { get; set; } = "draft";
        public DateTime TaoLuc { get; set; }
        public DateTime CapNhatLuc { get; set; }
        public Guid NguoiTaoId { get; set; }
        public int SoLuongCauHoi { get; set; }
        // Trả về danh sách câu hỏi chi tiết trong đề thi
        public List<CauHoiTrongDeThiDTO> CauHois { get; set; } = new();
    }

    /// <summary>
    /// DTO mô tả một câu hỏi có trong đề thi, bao gồm điểm số và thứ tự.
    /// </summary>
    public class CauHoiTrongDeThiDTO : CauHoiResponseDTO
    {
        public decimal Diem { get; set; }
        public int ThuTu { get; set; }
    }
    public class ThemCauHoiVaoDeThiDTO
    {
        [Required(ErrorMessage = "ID câu hỏi không được để trống.")]
        public Guid CauHoiId { get; set; }

        [Range(0.1, 10.0, ErrorMessage = "Điểm phải nằm trong khoảng từ 0.1 đến 10.")]
        public decimal Diem { get; set; } = 1.0m;
    }

    /// <summary>
    /// DTO cho yêu cầu cập nhật thông tin của một câu hỏi trong đề thi (ví dụ: điểm).
    /// </summary>
    public class CapNhatCauHoiTrongDeThiDTO
    {
        [Range(0.1, 10.0, ErrorMessage = "Điểm phải nằm trong khoảng từ 0.1 đến 10.")]
        public decimal? Diem { get; set; }

        // Có thể mở rộng để cập nhật cả thứ tự sau này
        // public int? ThuTu { get; set; }
    }

    /// <summary>
    /// DTO chứa kết quả thống kê chi tiết của một đề thi.
    /// </summary>
    public class DeThiThongKeDTO
    {
        public Guid DeThiId { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public int SoLuongNopBai { get; set; }
        public decimal DiemTrungBinh { get; set; }
        public decimal DiemCaoNhat { get; set; }
        public decimal DiemThapNhat { get; set; }
        public List<CauHoiThongKeDTO> ThongKeCauHoi { get; set; } = new();
    }

    /// <summary>
    /// DTO chứa thông tin thống kê cho một câu hỏi trong đề thi.
    /// </summary>
    public class CauHoiThongKeDTO
    {
        public Guid CauHoiId { get; set; }
        public string NoiDungCauHoi { get; set; } = string.Empty;
        public int ThuTuTrongDe { get; set; }
        public int SoLanTraLoiDung { get; set; }
        public int SoLanTraLoiSai { get; set; }
        /// <summary>
        /// Tỷ lệ trả lời đúng (từ 0.0 đến 1.0).
        /// </summary>
        public double TyLeTraLoiDung { get; set; }
    }

}