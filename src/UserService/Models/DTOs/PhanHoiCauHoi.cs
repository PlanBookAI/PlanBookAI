namespace TaskService.Models.DTOs
{
    public class PhanHoiCauHoi
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public CauHoiChiTietDto? DuLieu { get; set; }
    }

    public class CauHoiChiTietDto
    {
        public Guid Id { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public string Loai { get; set; } = string.Empty;
        public string DoKho { get; set; } = string.Empty;
        public string MonHoc { get; set; } = string.Empty;
        public string? ChuDe { get; set; }
        public string? LoiGiai { get; set; }
        public string? DapAnDung { get; set; }
        public List<LuaChonDto> CacLuaChon { get; set; } = new List<LuaChonDto>();
        public DateTime TaoLuc { get; set; }
        public DateTime CapNhatLuc { get; set; }
    }
}