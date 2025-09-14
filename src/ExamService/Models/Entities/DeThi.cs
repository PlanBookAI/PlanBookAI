using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExamService.Models.Enums;

namespace ExamService.Models.Entities
{
    [Table("exams", Schema = "assessment")]
    public class DeThi
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("title")]
        public string TieuDe { get; set; } = string.Empty;

        [Column("description")]
        public string? MoTa { get; set; }

        [Required]
        [Column("subject")]
        public string MonHoc { get; set; } = "HOA_HOC";

        [Column("grade")]
        public int? KhoiLop { get; set; }

        [Column("duration_minutes")]
        public int? ThoiGianLamBai { get; set; }

        [Column("total_score")]
        public decimal? DiemToiDa { get; set; }

        [Required]
        [Column("teacher_id")]
        public Guid NguoiTaoId { get; set; }
        
        [Column("instructions")]
        public string? HuongDan { get; set; }

        [Column("status")]
        public string TrangThai { get; set; } = "DRAFT";

        [Column("created_at")]
        public DateTime TaoLuc { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime CapNhatLuc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
        public virtual ICollection<BaiLam> BaiLams { get; set; }

        public DeThi()
        {
            ExamQuestions = new List<ExamQuestion>();
            BaiLams = new List<BaiLam>();
        }

        // Methods
        public decimal TinhDiemTrungBinh()
        {
            if (!BaiLams.Any()) return 0;
            return BaiLams.Average(bl => bl.KetQua?.Diem ?? 0);
        }

        public string LayThongTinTomTat()
        {
            return $"Đề thi: {TieuDe}\nMôn học: {MonHoc}\nKhối lớp: {KhoiLop}\nSố câu hỏi: {ExamQuestions.Count}\nThời gian: {ThoiGianLamBai} phút";
        }
    }
}