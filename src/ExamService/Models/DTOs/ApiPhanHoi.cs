namespace ExamService.Models.DTOs;

/// <summary>
/// Lớp phản hồi API chuẩn cho các endpoint
/// </summary>
public class ApiPhanHoi<T>
{
    /// <summary>
    /// Mã trạng thái HTTP
    /// </summary>
    public int MaTrangThai { get; set; } = 200;

    /// <summary>
    /// Thông báo kết quả
    /// </summary>
    public string ThongBao { get; set; } = "Thành công";

    /// <summary>
    /// Dữ liệu trả về
    /// </summary>
    public T? DuLieu { get; set; }

    /// <summary>
    /// Thời gian phản hồi
    /// </summary>
    public DateTime ThoiGian { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Tạo phản hồi thành công
    /// </summary>
    /// <param name="duLieu">Dữ liệu trả về</param>
    /// <param name="thongBao">Thông báo tùy chọn</param>
    /// <returns>Đối tượng phản hồi API</returns>
    public static ApiPhanHoi<T> ThanhCong(T duLieu, string thongBao = "Thành công")
    {
        return new ApiPhanHoi<T>
        {
            MaTrangThai = 200,
            ThongBao = thongBao,
            DuLieu = duLieu
        };
    }
    
    /// <summary>
    /// Tạo phản hồi thành công với dữ liệu
    /// </summary>
    /// <param name="duLieu">Dữ liệu trả về</param>
    /// <param name="thongBao">Thông báo tùy chọn</param>
    /// <returns>Đối tượng phản hồi API</returns>
    public static ApiPhanHoi<T> ThanhCongVoiDuLieu(T duLieu, string thongBao = "Thành công")
    {
        return new ApiPhanHoi<T>
        {
            MaTrangThai = 200,
            ThongBao = thongBao,
            DuLieu = duLieu
        };
    }
    
    /// <summary>
    /// Tạo phản hồi thất bại
    /// </summary>
    /// <param name="thongBao">Thông báo lỗi</param>
    /// <param name="maTrangThai">Mã trạng thái HTTP</param>
    /// <returns>Đối tượng phản hồi API</returns>
    public static ApiPhanHoi<T> ThatBai(string thongBao, int maTrangThai = 400)
    {
        return new ApiPhanHoi<T>
        {
            MaTrangThai = maTrangThai,
            ThongBao = thongBao,
            DuLieu = default
        };
    }

    /// <summary>
    /// Tạo phản hồi lỗi
    /// </summary>
    /// <param name="thongBao">Thông báo lỗi</param>
    /// <param name="maTrangThai">Mã trạng thái HTTP</param>
    /// <returns>Đối tượng phản hồi API</returns>
    public static ApiPhanHoi<T> Loi(string thongBao, int maTrangThai = 400)
    {
        return new ApiPhanHoi<T>
        {
            MaTrangThai = maTrangThai,
            ThongBao = thongBao,
            DuLieu = default
        };
    }
}

/// <summary>
/// Lớp phản hồi lỗi API chi tiết
/// </summary>
public class ApiPhanHoiLoi
{
    /// <summary>
    /// Mã lỗi
    /// </summary>
    public string MaLoi { get; set; } = "LOI_KHONG_XAC_DINH";

    /// <summary>
    /// Thông báo lỗi
    /// </summary>
    public string ThongBao { get; set; } = "Đã xảy ra lỗi không xác định";

    /// <summary>
    /// Chi tiết lỗi (chỉ trả về trong môi trường phát triển)
    /// </summary>
    public object? ChiTiet { get; set; }

    /// <summary>
    /// Thời gian phát sinh lỗi
    /// </summary>
    public DateTime ThoiGian { get; set; } = DateTime.UtcNow;
}