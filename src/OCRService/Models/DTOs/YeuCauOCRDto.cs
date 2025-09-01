using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OCRService.Models.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu upload ảnh bài làm để OCR
    /// </summary>
    public class YeuCauOCRDto
    {
        /// <summary>
        /// ID đề thi cần chấm
        /// </summary>
        [Required(ErrorMessage = "ID đề thi là bắt buộc")]
        public Guid DeThiId { get; set; }

        /// <summary>
        /// ID học sinh (có thể null nếu chưa biết)
        /// </summary>
        public Guid? HocSinhId { get; set; }

        /// <summary>
        /// ID bài làm (có thể null nếu tạo mới)
        /// </summary>
        public Guid? BaiLamId { get; set; }

        /// <summary>
        /// File ảnh bài làm
        /// </summary>
        [Required(ErrorMessage = "File ảnh bài làm là bắt buộc")]
        public IFormFile AnhBaiLam { get; set; } = null!;

        /// <summary>
        /// Loại bài làm (TRAC_NGHIEM, TU_LUAN, DUNG_SAI)
        /// </summary>
        [Required(ErrorMessage = "Loại bài làm là bắt buộc")]
        [StringLength(50, ErrorMessage = "Loại bài làm không được vượt quá 50 ký tự")]
        public string LoaiBaiLam { get; set; } = "TRAC_NGHIEM";

        /// <summary>
        /// Ghi chú thêm (nếu có)
        /// </summary>
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? GhiChu { get; set; }

        /// <summary>
        /// ID giáo viên (từ Gateway header)
        /// </summary>
        [Required(ErrorMessage = "ID giáo viên là bắt buộc")]
        public Guid GiaoVienId { get; set; }

        /// <summary>
        /// Thời gian tạo request
        /// </summary>
        public DateTime ThoiGianTao { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO cho response khi tạo yêu cầu OCR thành công
    /// </summary>
    public class PhanHoiTaoYeuCauOCRDto
    {
        /// <summary>
        /// Trạng thái thành công
        /// </summary>
        public bool ThanhCong { get; set; } = true;

        /// <summary>
        /// Thông báo
        /// </summary>
        public string ThongBao { get; set; } = "Đã tạo yêu cầu OCR thành công";

        /// <summary>
        /// ID yêu cầu OCR
        /// </summary>
        public Guid OcrRequestId { get; set; }

        /// <summary>
        /// ID bài làm
        /// </summary>
        public Guid BaiLamId { get; set; }

        /// <summary>
        /// Trạng thái hiện tại
        /// </summary>
        public string TrangThai { get; set; } = "PENDING";

        /// <summary>
        /// Thời gian ước tính xử lý (phút)
        /// </summary>
        public int ThoiGianUocTinh { get; set; } = 2;

        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime ThoiGianTao { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO cho response khi có lỗi
    /// </summary>
    public class PhanHoiLoiDto
    {
        /// <summary>
        /// Trạng thái thành công
        /// </summary>
        public bool ThanhCong { get; set; } = false;

        /// <summary>
        /// Thông báo lỗi
        /// </summary>
        public string ThongBao { get; set; } = string.Empty;

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public string MaLoi { get; set; } = string.Empty;

        /// <summary>
        /// Chi tiết lỗi
        /// </summary>
        public string? ChiTietLoi { get; set; }

        /// <summary>
        /// Thời gian xảy ra lỗi
        /// </summary>
        public DateTime ThoiGianLoi { get; set; } = DateTime.UtcNow;
    }
}

