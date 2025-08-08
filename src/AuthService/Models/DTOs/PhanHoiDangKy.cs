namespace AuthService.Models.DTOs
{
    public class PhanHoiDangKy
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public Guid? NguoiDungId { get; set; }
        public string? Email { get; set; }
        public string? HoTen { get; set; }
        public string? VaiTro { get; set; }
        public List<string> LoiValidation { get; set; } = new List<string>();
    }
}
