using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services
{
    // Lớp triển khai dịch vụ quản lý người dùng
    public class DichVuNguoiDung : IDichVuNguoiDung
    {
        private readonly IHoSoNguoiDungRepository _hoSoNguoiDungRepository;

        // Dependency Injection của repository
        public DichVuNguoiDung(IHoSoNguoiDungRepository hoSoNguoiDungRepository)
        {
            _hoSoNguoiDungRepository = hoSoNguoiDungRepository;
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng, có hỗ trợ phân trang.
        /// </summary>
        /// <param name="soTrang">Số trang hiện tại.</param>
        /// <param name="kichThuocTrang">Kích thước của mỗi trang.</param>
        /// <returns>Đối tượng PhanTrang chứa danh sách người dùng và thông tin phân trang.</returns>
        public async Task<IDichVuNguoiDung.PhanTrang<NguoiDung>> LayDanhSachNguoiDungAsync(int soTrang, int kichThuocTrang)
        {
            var tatCaNguoiDung = await _hoSoNguoiDungRepository.GetAllAsync();
            var nguoiDungPhanTrang = tatCaNguoiDung.Skip((soTrang - 1) * kichThuocTrang).Take(kichThuocTrang);

            return new IDichVuNguoiDung.PhanTrang<NguoiDung>
            {
                SoTrangHienTai = soTrang,
                TongSoTrang = (int)Math.Ceiling((double)tatCaNguoiDung.Count() / kichThuocTrang),
                TongSoMuc = tatCaNguoiDung.Count(),
                DuLieu = nguoiDungPhanTrang
            };
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng theo ID.
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <returns>Đối tượng NguoiDung.</returns>
        public async Task<NguoiDung> LayChiTietNguoiDungAsync(string id)
        {
            return await _hoSoNguoiDungRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật.</param>
        /// <param name="nguoiDungCapNhat">Đối tượng NguoiDung với thông tin mới.</param>
        public async Task CapNhatNguoiDungAsync(string id, NguoiDung nguoiDungCapNhat)
        {
            // Kiểm tra tính hợp lệ của dữ liệu trước khi cập nhật
            if (nguoiDungCapNhat == null || string.IsNullOrEmpty(nguoiDungCapNhat.HoTen))
            {
                throw new ArgumentException("Dữ liệu người dùng không hợp lệ.");
            }

            // Tìm người dùng hiện tại và cập nhật thông tin
            var nguoiDungHienTai = await _hoSoNguoiDungRepository.GetByIdAsync(id);
            if (nguoiDungHienTai == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy người dùng với ID: {id}");
            }

            nguoiDungHienTai.HoTen = nguoiDungCapNhat.HoTen;
            // Thêm các thuộc tính khác cần cập nhật tại đây

            await _hoSoNguoiDungRepository.UpdateAsync(nguoiDungHienTai);
        }

        /// <summary>
        /// Xóa một người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa.</param>
        public async Task XoaNguoiDungAsync(string id)
        {
            var nguoiDung = await _hoSoNguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy người dùng với ID: {id}");
            }
            await _hoSoNguoiDungRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Tải lên ảnh hồ sơ cho người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <param name="file">Tệp ảnh cần tải lên.</param>
        /// <returns>True nếu tải thành công, ngược lại là False.</returns>
        public async Task<bool> TaiAnhHoSoAsync(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            // Đây là logic giả lập. Trong thực tế, bạn sẽ cần:
            // 1. Lưu tệp vào một thư mục trên server hoặc dịch vụ lưu trữ đám mây (ví dụ: Azure Blob Storage, AWS S3).
            // 2. Cập nhật đường dẫn ảnh mới vào thuộc tính của người dùng trong cơ sở dữ liệu.

            var nguoiDung = await _hoSoNguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                return false;
            }

            // Giả lập lưu file và cập nhật đường dẫn ảnh
            string tenTep = $"{id}_{file.FileName}";
            string duongDanLuuTru = $"uploads/avatars/{tenTep}";

            // Cập nhật đường dẫn ảnh vào database
            // nguoiDung.AnhHoSo = duongDanLuuTru;
            // await _hoSoNguoiDungRepository.UpdateAsync(nguoiDung);

            return true;
        }
    }
}
