using System.Collections.Generic;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO chứa kết quả thống kê chung.
    /// </summary>
    public class ThongKeTongQuanDTO
    {
        public int TongSoCauHoi { get; set; }
        public int TongSoDeThi { get; set; }
        public List<ThongKeTheoNhomDTO> ThongKeTheoMonHoc { get; set; } = new();
        public List<ThongKeTheoNhomDTO> ThongKeTheoDoKho { get; set; } = new();
        public List<ThongKeTheoNhomDTO> ThongKeTheoChuDe { get; set; } = new();
    }

    /// <summary>
    /// DTO đại diện cho một nhóm thống kê (ví dụ: Môn học 'Hóa học' có 100 câu hỏi).
    /// </summary>
    public class ThongKeTheoNhomDTO
    {
        public string TenNhom { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }

    /// <summary>
    /// DTO chứa kết quả thống kê chi tiết chỉ về Câu hỏi.
    /// </summary>
    public class CauHoiThongKeTongQuanDTO
    {
        public int TongSoCauHoi { get; set; }
        public List<ThongKeTheoNhomDTO> ThongKeTheoMonHoc { get; set; } = new();
        public List<ThongKeTheoNhomDTO> ThongKeTheoDoKho { get; set; } = new();
        public List<ThongKeTheoNhomDTO> ThongKeTheoChuDe { get; set; } = new();
    }

    /// <summary>
    /// DTO chứa kết quả thống kê chi tiết chỉ về Đề thi.
    /// </summary>
    public class DeThiThongKeTongQuanDTO
    {
        public int TongSoDeThi { get; set; }
        public List<ThongKeTheoNhomDTO> ThongKeTheoTrangThai { get; set; } = new();
        public List<ThongKeTheoNhomDTO> ThongKeTheoMonHoc { get; set; } = new();
        public List<ThongKeTheoNhomDTO> ThongKeTheoKhoiLop { get; set; } = new();
    }

    /// <summary>
    /// DTO chứa kết quả thống kê về hoạt động của một người dùng cụ thể.
    /// </summary>
    public class NguoiDungThongKeDTO
    {
        public Guid UserId { get; set; }
        public int TongSoCauHoiDaTao { get; set; }
        public int TongSoDeThiDaTao { get; set; }
        public int TongSoMauDeThiDaTao { get; set; }
    }
}