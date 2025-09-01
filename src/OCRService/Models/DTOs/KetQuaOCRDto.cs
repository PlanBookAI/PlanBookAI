using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OCRService.Models.DTOs
{
    /// <summary>
    /// DTO cho kết quả OCR
    /// </summary>
    public class KetQuaOCRDto
    {
        /// <summary>
        /// ID yêu cầu OCR
        /// </summary>
        public Guid OcrRequestId { get; set; }

        /// <summary>
        /// ID bài làm
        /// </summary>
        public Guid BaiLamId { get; set; }

        /// <summary>
        /// ID đề thi
        /// </summary>
        public Guid DeThiId { get; set; }

        /// <summary>
        /// Thông tin học sinh
        /// </summary>
        public ThongTinHocSinhDto? ThongTinHocSinh { get; set; }

        /// <summary>
        /// Danh sách đáp án
        /// </summary>
        public List<DapAnDto> DapAn { get; set; } = new List<DapAnDto>();

        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public string TrangThai { get; set; } = "PENDING";

        /// <summary>
        /// Độ chính xác OCR (0.00 - 1.00)
        /// </summary>
        [Range(0.00, 1.00, ErrorMessage = "Độ chính xác phải từ 0.00 đến 1.00")]
        public decimal DoChinhXac { get; set; }

        /// <summary>
        /// Thời gian xử lý (milliseconds)
        /// </summary>
        public int ThoiGianXuLy { get; set; }

        /// <summary>
        /// AI provider sử dụng
        /// </summary>
        public string AIProvider { get; set; } = string.Empty;

        /// <summary>
        /// Điểm số (nếu đã chấm)
        /// </summary>
        public decimal? Diem { get; set; }

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime ThoiGianTao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời gian hoàn thành
        /// </summary>
        public DateTime? ThoiGianHoanThanh { get; set; }

        /// <summary>
        /// Ghi chú (nếu có)
        /// </summary>
        public string? GhiChu { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin học sinh
    /// </summary>
    public class ThongTinHocSinhDto
    {
        /// <summary>
        /// ID học sinh
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tên học sinh
        /// </summary>
        [Required(ErrorMessage = "Tên học sinh là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên học sinh không được vượt quá 255 ký tự")]
        public string TenHocSinh { get; set; } = string.Empty;

        /// <summary>
        /// Mã số học sinh
        /// </summary>
        [StringLength(50, ErrorMessage = "Mã số học sinh không được vượt quá 50 ký tự")]
        public string? MaSoHocSinh { get; set; }

        /// <summary>
        /// Lớp
        /// </summary>
        [StringLength(20, ErrorMessage = "Lớp không được vượt quá 20 ký tự")]
        public string? Lop { get; set; }

        /// <summary>
        /// Khối
        /// </summary>
        public int? Khoi { get; set; }
    }

    /// <summary>
    /// DTO cho đáp án từng câu hỏi
    /// </summary>
    public class DapAnDto
    {
        /// <summary>
        /// Số thứ tự câu hỏi
        /// </summary>
        [Required(ErrorMessage = "Số thứ tự câu hỏi là bắt buộc")]
        public int CauHoi { get; set; }

        /// <summary>
        /// Đáp án học sinh chọn
        /// </summary>
        [Required(ErrorMessage = "Đáp án là bắt buộc")]
        [StringLength(10, ErrorMessage = "Đáp án không được vượt quá 10 ký tự")]
        public string DapAn { get; set; } = string.Empty;

        /// <summary>
        /// Độ tin cậy của OCR cho câu này (0.00 - 1.00)
        /// </summary>
        [Range(0.00, 1.00, ErrorMessage = "Độ tin cậy phải từ 0.00 đến 1.00")]
        public decimal DoTinCay { get; set; }

        /// <summary>
        /// Điểm của câu hỏi này
        /// </summary>
        public decimal? Diem { get; set; }

        /// <summary>
        /// Đáp án đúng (từ đề thi)
        /// </summary>
        public string? DapAnDung { get; set; }

        /// <summary>
        /// Có đúng hay không
        /// </summary>
        public bool? Dung { get; set; }
    }

    /// <summary>
    /// DTO cho response khi lấy kết quả OCR
    /// </summary>
    public class PhanHoiKetQuaOCRDto
    {
        /// <summary>
        /// Trạng thái thành công
        /// </summary>
        public bool ThanhCong { get; set; } = true;

        /// <summary>
        /// Thông báo
        /// </summary>
        public string ThongBao { get; set; } = "Lấy kết quả OCR thành công";

        /// <summary>
        /// Dữ liệu kết quả OCR
        /// </summary>
        public KetQuaOCRDto? DuLieu { get; set; }

        /// <summary>
        /// Thời gian trả về
        /// </summary>
        public DateTime ThoiGianTraVe { get; set; } = DateTime.UtcNow;
    }
}

