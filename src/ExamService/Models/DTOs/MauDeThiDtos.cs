using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tạo hoặc cập nhật Mẫu đề thi.
    /// </summary>
    public class MauDeThiRequestDTO
    {
        [Required]
        public string TieuDe { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        [Required]
        public string MonHoc { get; set; } = string.Empty;
        [Range(10, 12)]
        public int KhoiLop { get; set; }
        [Required]
        [MinLength(1)]
        public List<YeuCauCauHoiDTO> CauTruc { get; set; } = new();
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
        public int KhoiLop { get; set; }
        public List<YeuCauCauHoiDTO> CauTruc { get; set; } = new();
        public Guid NguoiTaoId { get; set; }
        public DateTime CapNhatLuc { get; set; }
    }
}