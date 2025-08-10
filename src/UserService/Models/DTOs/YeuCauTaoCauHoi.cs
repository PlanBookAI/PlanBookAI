using System.ComponentModel.DataAnnotations;
using TaskService.Models.Enums;

namespace TaskService.Models.DTOs
{
    public class YeuCauTaoCauHoi
    {
        [Required(ErrorMessage = "Nội dung câu hỏi không được để trống.")]
        public string NoiDung { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại câu hỏi là bắt buộc.")]
        [EnumDataType(typeof(LoaiCauHoi), ErrorMessage = "Loại câu hỏi không hợp lệ.")]
        public LoaiCauHoi Loai { get; set; } = LoaiCauHoi.MULTIPLE_CHOICE;

        [Required(ErrorMessage = "Mức độ khó là bắt buộc.")]
        [EnumDataType(typeof(MucDoKho), ErrorMessage = "Mức độ khó không hợp lệ.")]
        public MucDoKho DoKho { get; set; } = MucDoKho.MEDIUM;

        [Required(ErrorMessage = "Môn học không được để trống.")]
        [StringLength(50)]
        public string MonHoc { get; set; } = "HoaHoc";

        [StringLength(255)]
        public string? ChuDe { get; set; }

        public string? LoiGiai { get; set; }

        // Dành cho câu hỏi trắc nghiệm
        public List<LuaChonDto>? CacLuaChon { get; set; }

        [StringLength(1, ErrorMessage = "Đáp án đúng chỉ được là một ký tự (A, B, C, D).")]
        public string? DapAnDung { get; set; }
    }

    public class LuaChonDto
    {
        [Required]
        public char ThuTu { get; set; } // A, B, C, D

        [Required]
        public string NoiDung { get; set; } = string.Empty;
    }
}