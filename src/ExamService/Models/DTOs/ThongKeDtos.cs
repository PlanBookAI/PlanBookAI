using System;
using System.Collections.Generic;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO chứa kết quả báo cáo thống kê tổng quan của một giáo viên.
    /// Đây là DTO cấp cao nhất, bao gồm tất cả các thông tin thống kê.
    /// </summary>
    public class BaoCaoThongKeGiaoVienDTO
    {
        /// <summary>
        /// ID của giáo viên được thống kê
        /// </summary>
        public Guid TeacherId { get; set; }
        
        /// <summary>
        /// Thời điểm tạo báo cáo
        /// </summary>
        public DateTime ThoiDiemBaoCao { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Thống kê về ngân hàng câu hỏi
        /// </summary>
        public ThongKeTongQuanCauHoiDTO ThongKeCauHoi { get; set; } = new();
        
        /// <summary>
        /// Thống kê về đề thi
        /// </summary>
        public ThongKeTongQuanDeThiDTO ThongKeDeThi { get; set; } = new();
        
        /// <summary>
        /// Thống kê về hoạt động của giáo viên
        /// </summary>
        public ThongKeHoatDongDTO ThongKeHoatDong { get; set; } = new();
    }

    /// <summary>
    /// DTO chứa thống kê chi tiết chỉ về Câu hỏi.
    /// </summary>
    public class ThongKeTongQuanCauHoiDTO
    {
        /// <summary>
        /// Tổng số câu hỏi
        /// </summary>
        public int TongSo { get; set; }
        
        /// <summary>
        /// Thống kê câu hỏi theo môn học
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoMonHoc { get; set; } = new();
        
        /// <summary>
        /// Thống kê câu hỏi theo độ khó
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoDoKho { get; set; } = new();
        
        /// <summary>
        /// Thống kê câu hỏi theo chủ đề
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoChuDe { get; set; } = new();
        
        /// <summary>
        /// Thống kê câu hỏi theo loại
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoLoai { get; set; } = new();
    }

    /// <summary>
    /// DTO chứa thống kê chi tiết chỉ về Đề thi.
    /// </summary>
    public class ThongKeTongQuanDeThiDTO
    {
        /// <summary>
        /// Tổng số đề thi
        /// </summary>
        public int TongSo { get; set; }
        
        /// <summary>
        /// Thống kê đề thi theo trạng thái
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoTrangThai { get; set; } = new();
        
        /// <summary>
        /// Thống kê đề thi theo môn học
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoMonHoc { get; set; } = new();
        
        /// <summary>
        /// Thống kê đề thi theo khối lớp
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoKhoiLop { get; set; } = new();
        
        /// <summary>
        /// Thống kê đề thi theo số lượng câu hỏi
        /// </summary>
        public ThongKePhanPhoi TheoSoLuongCauHoi { get; set; } = new();
    }
    
    /// <summary>
    /// DTO chứa thống kê về hoạt động của giáo viên
    /// </summary>
    public class ThongKeHoatDongDTO
    {
        /// <summary>
        /// Ngày hoạt động gần nhất
        /// </summary>
        public DateTime? HoatDongGanNhat { get; set; }
        
        /// <summary>
        /// Thống kê hoạt động theo ngày
        /// </summary>
        public List<ThongKeTheoNgayDTO> TheoNgay { get; set; } = new();
        
        /// <summary>
        /// Thống kê hoạt động theo loại
        /// </summary>
        public List<ThongKeTheoNhomDTO> TheoLoaiHoatDong { get; set; } = new();
    }
    
    /// <summary>
    /// DTO chứa thống kê theo ngày
    /// </summary>
    public class ThongKeTheoNgayDTO
    {
        /// <summary>
        /// Ngày thống kê
        /// </summary>
        public DateTime Ngay { get; set; }
        
        /// <summary>
        /// Số lượng hoạt động trong ngày
        /// </summary>
        public int SoLuongHoatDong { get; set; }
    }

    /// <summary>
    /// DTO chung để biểu diễn một nhóm thống kê (Tên nhóm và số lượng).
    /// Ví dụ: { TenNhom: "Hóa học", SoLuong: 150 }
    /// </summary>
    public class ThongKeTheoNhomDTO
    {
        /// <summary>
        /// Tên nhóm thống kê
        /// </summary>
        public string TenNhom { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lượng trong nhóm
        /// </summary>
        public int SoLuong { get; set; }
        
        /// <summary>
        /// Tỷ lệ phần trăm (nếu có)
        /// </summary>
        public double? TyLePhanTram { get; set; }
    }
    
    /// <summary>
    /// DTO chứa thông tin về phân phối thống kê
    /// </summary>
    public class ThongKePhanPhoi
    {
        /// <summary>
        /// Giá trị nhỏ nhất
        /// </summary>
        public double Min { get; set; }
        
        /// <summary>
        /// Giá trị lớn nhất
        /// </summary>
        public double Max { get; set; }
        
        /// <summary>
        /// Giá trị trung bình
        /// </summary>
        public double TrungBinh { get; set; }
        
        /// <summary>
        /// Giá trị trung vị
        /// </summary>
        public double TrungVi { get; set; }
        
        /// <summary>
        /// Các khoảng giá trị và số lượng
        /// </summary>
        public List<ThongKeTheoNhomDTO> PhanPhoi { get; set; } = new();
    }
    
    /// <summary>
    /// DTO chứa thông tin chi tiết về một đề thi cho báo cáo thống kê
    /// </summary>
    public class DeThiThongKeChiTietDTO
    {
        /// <summary>
        /// ID của đề thi
        /// </summary>
        public Guid DeThiId { get; set; }
        
        /// <summary>
        /// Tiêu đề đề thi
        /// </summary>
        public string TieuDe { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lượng học sinh đã nộp bài
        /// </summary>
        public int SoLuongNopBai { get; set; }
        
        /// <summary>
        /// Điểm trung bình của đề thi
        /// </summary>
        public decimal DiemTrungBinh { get; set; }
        
        /// <summary>
        /// Điểm cao nhất của đề thi
        /// </summary>
        public decimal DiemCaoNhat { get; set; }
        
        /// <summary>
        /// Điểm thấp nhất của đề thi
        /// </summary>
        public decimal DiemThapNhat { get; set; }
        
        /// <summary>
        /// Thống kê chi tiết từng câu hỏi
        /// </summary>
        public List<CauHoiThongKeChiTietDTO> ChiTietCauHoi { get; set; } = new();
    }
    
    /// <summary>
    /// DTO chứa thông tin thống kê chi tiết về một câu hỏi trong đề thi
    /// </summary>
    public class CauHoiThongKeChiTietDTO
    {
        /// <summary>
        /// ID của câu hỏi
        /// </summary>
        public Guid CauHoiId { get; set; }
        
        /// <summary>
        /// Nội dung câu hỏi
        /// </summary>
        public string NoiDungCauHoi { get; set; } = string.Empty;
        
        /// <summary>
        /// Thứ tự câu hỏi trong đề thi
        /// </summary>
        public int ThuTuTrongDe { get; set; }
        
        /// <summary>
        /// Số lần trả lời đúng
        /// </summary>
        public int SoLanTraLoiDung { get; set; }
        
        /// <summary>
        /// Tỷ lệ trả lời đúng (phần trăm)
        /// </summary>
        public double TyLeTraLoiDung { get; set; }
        
        /// <summary>
        /// Độ khó thực tế của câu hỏi (dựa trên tỷ lệ trả lời đúng)
        /// </summary>
        public string DoKhoThucTe { get; set; } = string.Empty;
    }
}