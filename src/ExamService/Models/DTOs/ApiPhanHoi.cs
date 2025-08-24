namespace ExamService.Models.DTOs
{
    public class ApiPhanHoi<T>
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = string.Empty;
        public T? DuLieu { get; set; }
        public List<string>? Loi { get; set; }

        public static ApiPhanHoi<T> ThanhCongVoiDuLieu(T data, string message = "Thực hiện thành công")
        {
            return new ApiPhanHoi<T> { ThanhCong = true, DuLieu = data, ThongBao = message };
        }

        public static ApiPhanHoi<T> ThatBai(string message, List<string>? errors = null)
        {
            return new ApiPhanHoi<T> { ThanhCong = false, ThongBao = message, Loi = errors };
        }
    }
}