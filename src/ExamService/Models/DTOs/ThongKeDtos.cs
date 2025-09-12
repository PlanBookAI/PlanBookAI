using System.Collections.Generic;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO chứa kết quả báo cáo thống kê tổng quan của một giáo viên.
    /// Đây là DTO cấp cao nhất, bao gồm tất cả các thông tin thống kê.
    /// </summary>
    public class BaoCaoThongKeGiaoVienDTO
    {
        public Guid TeacherId { get; set; }
        public ThongKeTongQuanCauHoiDTO ThongKeCauHoi { get; set; } = new();
        public ThongKeTongQuanDeThiDTO ThongKeDeThi { get; set; } = new();
    }

    /// <summary>
    /// DTO chứa thống kê chi tiết chỉ về Câu hỏi.
    /// </summary>
    public class ThongKeTongQuanCauHoiDTO
    {
        public int TongSo { get; set; }
        public List<ThongKeTheoNhomDTO> TheoMonHoc { get; set; } = new();
        public List<ThongKeTheoNhomDTO> TheoDoKho { get; set; } = new();
        public List<ThongKeTheoNhomDTO> TheoChuDe { get; set; } = new();
    }

    /// <summary>
    /// DTO chứa thống kê chi tiết chỉ về Đề thi.
    /// </summary>
    public class ThongKeTongQuanDeThiDTO
    {
        public int TongSo { get; set; }
        public List<ThongKeTheoNhomDTO> TheoTrangThai { get; set; } = new();
        public List<ThongKeTheoNhomDTO> TheoMonHoc { get; set; } = new();
        public List<ThongKeTheoNhomDTO> TheoKhoiLop { get; set; } = new();
    }

    /// <summary>
    /// DTO chung để biểu diễn một nhóm thống kê (Tên nhóm và số lượng).
    /// Ví dụ: { TenNhom: "Hóa học", SoLuong: 150 }
    /// </summary>
    public class ThongKeTheoNhomDTO
    {
        public string TenNhom { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }
}