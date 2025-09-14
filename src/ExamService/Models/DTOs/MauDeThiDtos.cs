using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tạo hoặc cập nhật Mẫu đề thi.
    /// </summary>
    public class MauDeThiRequestDTO
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5 đến 255 ký tự")]
        public string TieuDe { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
        
        [Required(ErrorMessage = "Môn học không được để trống")]
        public string MonHoc { get; set; } = "HOA_HOC";
        
        [Range(10, 12, ErrorMessage = "Khối lớp phải từ 10 đến 12")]
        public int? KhoiLop { get; set; }
        
        [Range(15, 300, ErrorMessage = "Thời gian làm bài phải từ 15 đến 300 phút")]
        public int? ThoiGianLam { get; set; }
        
        [Range(1, 100, ErrorMessage = "Tổng điểm phải từ 1 đến 100")]
        public decimal? TongDiem { get; set; }
        
        /// <summary>
        /// Cấu trúc đề thi dạng Dictionary: {"easy": 5, "medium": 10, "hard": 3, "topics": ["topic1", "topic2"]}
        /// Hoặc dạng Array: [{"chuDe": "topic1", "doKho": "EASY", "soLuong": 5}]
        /// </summary>
        public object? CauTruc { get; set; }
    }

    /// <summary>
    /// DTO trả về thông tin Mẫu đề thi.
    /// </summary>
    public class MauDeThiResponseDTO
    {
        public Guid Id { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public string MonHoc { get; set; } = string.Empty;
        public int? KhoiLop { get; set; }
        public int? ThoiGianLam { get; set; }
        public decimal? TongDiem { get; set; }
        public object CauTruc { get; set; } = new();
        public string TrangThai { get; set; } = "ACTIVE";
        public Guid NguoiTaoId { get; set; }
        public DateTime TaoLuc { get; set; }
        public DateTime CapNhatLuc { get; set; }
    }

    /// <summary>
    /// DTO cho yêu cầu sao chép mẫu đề thi
    /// </summary>
    public class SaoChepMauDeThiDTO
    {
        [Required(ErrorMessage = "Tiêu đề mới không được để trống")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5 đến 255 ký tự")]
        public string TieuDeMoi { get; set; } = string.Empty;
        
        public string? MoTaMoi { get; set; }
    }
}