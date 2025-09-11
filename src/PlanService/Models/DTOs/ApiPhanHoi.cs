namespace PlanService.Models.DTOs
{
    public class ApiPhanHoi<T>
    {
        public bool ThanhCong { get; set; }
        public string ThongDiep { get; set; } = "";
        public T? DuLieu { get; set; }
        public object? Loi{ get; set; }

        public static ApiPhanHoi<T> ThanhCongOk(T data, string msg = "Thành công")
            => new() { ThanhCong = true, ThongDiep = msg, DuLieu = data };

        public static ApiPhanHoi<T> ThatBai(string msg, object? loi = null)
            => new() { ThanhCong = false, ThongDiep = msg, Loi = loi };
    }
}
