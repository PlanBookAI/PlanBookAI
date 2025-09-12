namespace PlanbookAI.FileStorageService.Models.DTOs
{
    public class YeuCauUploadTepTin
    {
        public IFormFile Tep { get; set; } = null!;
        public string? Loai { get; set; }
        public string? Metadata { get; set; }
    }
    
    public class PhanHoiTepTin
    {
        public Guid Id { get; set; }
        public string TenGoc { get; set; } = string.Empty;
        public string? DuongDan { get; set; }
        public long KichThuoc { get; set; }
        public string MimeType { get; set; } = string.Empty;
        public string Loai { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string? Hash { get; set; }
        public DateTime NgayTao { get; set; }
        public Dictionary<string, string>? ThuocTinh { get; set; }
        public bool? IsDuplicate { get; set; }
    }
    
    public class PhanHoiDanhSachTepTin
    {
        public List<PhanHoiTepTin> DanhSach { get; set; } = new();
        public int TongSo { get; set; }
        public int Trang { get; set; }
        public int KichThuoc { get; set; }
        public int TongTrang { get; set; }
    }
    
    public class YeuCauCapNhatMetadata
    {
        public Dictionary<string, string> ThuocTinh { get; set; } = new();
    }
    
    public class PhanHoiLoi
    {
        public string ThongBao { get; set; } = string.Empty;
        public string? ChiTiet { get; set; }
    }
}
