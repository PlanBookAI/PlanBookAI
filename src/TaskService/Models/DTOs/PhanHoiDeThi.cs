using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskService.Models.DTOs
{
    public class PhanHoiDeThi
    {
        public Guid Id { get; set; }
        public Guid DeThiId { get; set; }
        public Guid HocSinhId { get; set; }
        public decimal Diem { get; set; }
        public decimal DiemToiDa { get; set; }
        public decimal TyLe { get; set; }
        public int ThoiGianLam { get; set; }
        public DateTime NopLuc { get; set; }
        public List<CauTraLoi> CauTraLois { get; set; } = new();
    }

    public class CauTraLoi
    {
        public Guid Id { get; set; }
        public Guid CauHoiId { get; set; }
        public string DapAnHocSinh { get; set; } = string.Empty;
        public bool LaDung { get; set; }
        public decimal Diem { get; set; }
        public string? NhanXet { get; set; }
    }

    public class YeuCauTaoDeThi
    {
        [Required]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        public string MonHoc { get; set; } = string.Empty;

        [Required]
        [Range(10, 12)]
        public int KhoiLop { get; set; }

        [Required]
        [Range(15, 180)]
        public int ThoiGianLamBai { get; set; }

        [Required]
        [Range(1, 100)]
        public int SoCauHoi { get; set; }

        public string? DoKho { get; set; }
        public string? ChuDe { get; set; }
        public string? HuongDan { get; set; }
    }

    public class YeuCauNopBai
    {
        [Required]
        public Guid DeThiId { get; set; }

        [Required]
        public Guid HocSinhId { get; set; }

        [Required]
        public Dictionary<Guid, string> DapAn { get; set; } = new();

        public int ThoiGianLam { get; set; }
    }
}