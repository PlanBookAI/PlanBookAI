using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Services
{
    // Giao diện định nghĩa các dịch vụ quản lý người dùng
    public interface IDichVuNguoiDung
    {
        // Lấy danh sách tất cả người dùng, có hỗ trợ phân trang
        Task<PhanTrang<NguoiDung>> LayDanhSachNguoiDungAsync(int soTrang, int kichThuocTrang);

        // Lấy thông tin chi tiết của một người dùng theo ID
        Task<NguoiDung> LayChiTietNguoiDungAsync(string id);

        // Cập nhật thông tin người dùng
        Task CapNhatNguoiDungAsync(string id, NguoiDung nguoiDungCapNhat);

        // Xóa một người dùng
        Task XoaNguoiDungAsync(string id);

        // Tải lên ảnh hồ sơ cho người dùng
        Task<bool> TaiAnhHoSoAsync(string id, IFormFile file);

        // Một class giả định cho phân trang
        public class PhanTrang<T>
        {
            public int SoTrangHienTai { get; set; }
            public int TongSoTrang { get; set; }
            public int TongSoMuc { get; set; }
            public IEnumerable<T> DuLieu { get; set; }
        }
    }
}

