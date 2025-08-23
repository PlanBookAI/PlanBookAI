using System.ComponentModel.DataAnnotations;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO trả về thông tin của một bản ghi liên kết giữa Đề thi và Câu hỏi.
    /// </summary>
    public class DeThiCauHoiResponseDTO
    {
        public Guid Id { get; set; } // ID của bản ghi ExamQuestion
        public Guid DeThiId { get; set; }
        public int ThuTu { get; set; }
        public decimal Diem { get; set; }
        public CauHoiResponseDTO CauHoi { get; set; } // Thông tin chi tiết câu hỏi
    }

    /// <summary>
    /// DTO cho yêu cầu cập nhật thứ tự của một câu hỏi trong đề thi.
    /// </summary>
    public class CapNhatThuTuDTO
    {
        [Required(ErrorMessage = "Thứ tự mới không được để trống.")]
        [Range(1, 200, ErrorMessage = "Thứ tự phải là một số dương.")]
        public int ThuTuMoi { get; set; }
    }

    /// <summary>
    /// DTO cho yêu cầu cập nhật điểm của một câu hỏi trong đề thi.
    /// </summary>
    public class CapNhatDiemDTO
    {
        [Required(ErrorMessage = "Điểm mới không được để trống.")]
        [Range(0.0, 10.0, ErrorMessage = "Điểm phải nằm trong khoảng từ 0.0 đến 10.0.")]
        public decimal DiemMoi { get; set; }
    }
}