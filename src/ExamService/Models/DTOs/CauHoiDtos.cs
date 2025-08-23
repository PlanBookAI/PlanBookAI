using System.ComponentModel.DataAnnotations;

namespace ExamService.Models.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tạo hoặc cập nhật một câu hỏi.
    /// </summary>
    public class CauHoiRequestDTO
    {
        [Required(ErrorMessage = "Nội dung câu hỏi không được để trống.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Nội dung câu hỏi phải từ 10 đến 2000 ký tự.")]
        public string NoiDung { get; set; } = string.Empty;

        [Required(ErrorMessage = "Môn học không được để trống.")]
        public string MonHoc { get; set; } = string.Empty;

        public string LoaiCauHoi { get; set; } = "multiple_choice";
        public string? ChuDe { get; set; }
        public string DoKho { get; set; } = "medium";

        [Required(ErrorMessage = "Đáp án đúng không được để trống.")]
        public string DapAnDung { get; set; } = string.Empty;

        public string? GiaiThich { get; set; }

        // Danh sách các lựa chọn cho câu hỏi trắc nghiệm
        public List<LuaChonRequestDTO> LuaChons { get; set; } = new List<LuaChonRequestDTO>();
    }

    /// <summary>
    /// DTO cho một lựa chọn trong câu hỏi.
    /// </summary>
    public class LuaChonRequestDTO
    {
        [Required(ErrorMessage = "Nội dung lựa chọn không được để trống.")]
        public string NoiDung { get; set; } = string.Empty;
        public bool LaDapAnDung { get; set; }
    }

    /// <summary>
    /// DTO trả về thông tin chi tiết của một câu hỏi.
    /// </summary>
    public class CauHoiResponseDTO
    {
        public Guid Id { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public string MonHoc { get; set; } = string.Empty;
        public string LoaiCauHoi { get; set; } = "multiple_choice";
        public string? ChuDe { get; set; }
        public string DoKho { get; set; } = "medium";
        public string DapAnDung { get; set; } = string.Empty;
        public string? GiaiThich { get; set; }
        public DateTime TaoLuc { get; set; }
        public DateTime CapNhatLuc { get; set; }
        public Guid NguoiTaoId { get; set; }
        public List<LuaChonResponseDTO> LuaChons { get; set; } = new List<LuaChonResponseDTO>();
    }

    /// <summary>
    /// DTO trả về thông tin của một lựa chọn.
    /// </summary>
    public class LuaChonResponseDTO
    {
        public Guid Id { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool LaDapAnDung { get; set; }
    }

    /// <summary>
    /// DTO chứa các tham số để tìm kiếm, lọc và phân trang câu hỏi.
    /// </summary>
    public class CauHoiSearchParametersDTO : PagingDTO // Kế thừa PagingDTO để có sẵn phân trang
    {
        /// <summary>
        /// Từ khóa tìm kiếm trong nội dung câu hỏi.
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Lọc theo môn học.
        /// </summary>
        public string? MonHoc { get; set; }

        /// <summary>
        /// Lọc theo chủ đề.
        /// </summary>
        public string? ChuDe { get; set; }

        /// <summary>
        /// Lọc theo độ khó (ví dụ: easy, medium, hard).
        /// </summary>
        public string? DoKho { get; set; }
    }

    /// <summary>
    /// DTO chứa kết quả sau khi import câu hỏi từ file Excel.
    /// </summary>
    public class ImportResultDTO
    {
        public int TotalRows { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO cho yêu cầu tạo một lựa chọn mới và gắn nó vào một câu hỏi.
    /// </summary>
    public class TaoLuaChonRequestDTO : LuaChonRequestDTO // Kế thừa từ DTO đã có
    {
        [Required(ErrorMessage = "ID của câu hỏi cha không được để trống.")]
        public Guid CauHoiId { get; set; }
    }

}