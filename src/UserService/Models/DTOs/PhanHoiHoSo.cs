namespace UserService.Models.DTOs
{
    public class PhanHoiHoSo
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public ThongTinNguoiDungDto? DuLieu { get; set; }
    }
}