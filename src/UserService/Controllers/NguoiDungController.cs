using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Models.Entities;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/v1/nguoi-dung")]
    public class NguoiDungController : ControllerBase
    {
        private readonly ILogger<NguoiDungController> _logger;

        // ---- MOCK DATABASE ----
        // Trong thực tế, đây sẽ là DbContext được inject vào.
        // ID này phải khớp với ID trong mock token để test.
        private static readonly Guid mockTeacherId = new Guid("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6");
        private static readonly List<HoSoNguoiDung> _mockHoSoDb = new List<HoSoNguoiDung>
        {
            new HoSoNguoiDung
            {
                UserId = mockTeacherId,
                HoTen = "Giáo Viên Test",
                SoDienThoai = "0987654321",
                DiaChi = "123 Đường ABC, Quận 1, TPHCM",
                NgaySinh = new DateTime(1990, 5, 15),
                AnhDaiDienUrl = "https://example.com/avatar.jpg",
                MoTaBanThan = "Giáo viên Hóa học với 10 năm kinh nghiệm.",
                TaoLuc = DateTime.UtcNow.AddDays(-10),
                CapNhatLuc = DateTime.UtcNow.AddDays(-1)
            }
        };
        // ---- END MOCK DATABASE ----

        public NguoiDungController(ILogger<NguoiDungController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Lấy thông tin hồ sơ của người dùng đang đăng nhập.
        /// </summary>
        /// <returns>Thông tin chi tiết hồ sơ người dùng.</returns>
        [HttpGet("thong-tin")]
        public IActionResult GetThongTin()
        {
            // Lấy thông tin từ header do Gateway chuyển tiếp
            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdStr) ||
                !Guid.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("GetThongTin: Không thể lấy User ID từ header.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            Request.Headers.TryGetValue("X-User-Role", out var vaiTro);
            Request.Headers.TryGetValue("X-User-Email", out var email); // Giả sử Gateway cũng gửi email

            _logger.LogInformation("Nhận yêu cầu lấy thông tin cho User ID: {UserId}", userId);

            var hoSo = _mockHoSoDb.FirstOrDefault(p => p.UserId == userId);

            if (hoSo == null)
            {
                _logger.LogWarning("Không tìm thấy hồ sơ cho User ID: {UserId}", userId);
                return NotFound(new { message = "Không tìm thấy hồ sơ người dùng." });
            }

            var dto = new ThongTinNguoiDungDto
            {
                Id = hoSo.UserId,
                Email = email.ToString() ?? "N/A", // Lấy email từ header
                HoTen = hoSo.HoTen,
                SoDienThoai = hoSo.SoDienThoai,
                NgaySinh = hoSo.NgaySinh,
                DiaChi = hoSo.DiaChi,
                AnhDaiDienUrl = hoSo.AnhDaiDienUrl,
                MoTaBanThan = hoSo.MoTaBanThan,
                VaiTro = vaiTro.ToString() ?? "N/A", // Lấy vai trò từ header
                CapNhatLuc = hoSo.CapNhatLuc
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ của người dùng đang đăng nhập.
        /// </summary>
        /// <param name="yeuCau">Dữ liệu cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("cap-nhat")]
        public IActionResult CapNhatHoSo([FromBody] YeuCauCapNhatHoSo yeuCau)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdStr) ||
                !Guid.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("CapNhatHoSo: Không thể lấy User ID từ header.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            _logger.LogInformation("Nhận yêu cầu cập nhật hồ sơ cho User ID: {UserId}", userId);

            var hoSo = _mockHoSoDb.FirstOrDefault(p => p.UserId == userId);
            if (hoSo == null)
            {
                _logger.LogWarning("Không tìm thấy hồ sơ để cập nhật cho User ID: {UserId}", userId);
                return NotFound(new { message = "Không tìm thấy hồ sơ người dùng." });
            }

            // Cập nhật dữ liệu từ DTO vào entity
            hoSo.HoTen = yeuCau.HoTen;
            hoSo.SoDienThoai = yeuCau.SoDienThoai;
            hoSo.NgaySinh = yeuCau.NgaySinh;
            hoSo.DiaChi = yeuCau.DiaChi;
            hoSo.AnhDaiDienUrl = yeuCau.AnhDaiDienUrl;
            hoSo.MoTaBanThan = yeuCau.MoTaBanThan;
            hoSo.CapNhatLuc = DateTime.UtcNow;

            // TODO: Ghi lại LichSuHoatDong
            _logger.LogInformation("Đã cập nhật thành công hồ sơ cho User ID: {UserId}", userId);

            // Tạo response
            var phanHoi = new PhanHoiHoSo
            {
                ThanhCong = true,
                ThongBao = "Cập nhật hồ sơ thành công!",
                DuLieu = new ThongTinNguoiDungDto
                {
                    Id = hoSo.UserId,
                    HoTen = hoSo.HoTen,
                    SoDienThoai = hoSo.SoDienThoai,
                    NgaySinh = hoSo.NgaySinh,
                    DiaChi = hoSo.DiaChi,
                    AnhDaiDienUrl = hoSo.AnhDaiDienUrl,
                    MoTaBanThan = hoSo.MoTaBanThan,
                    CapNhatLuc = hoSo.CapNhatLuc,
                    VaiTro = Request.Headers["X-User-Role"].ToString() ?? "N/A",
                    Email = Request.Headers["X-User-Email"].ToString() ?? "N/A"
                }
            };

            return Ok(phanHoi);
        }

        /// <summary>
        /// Lấy danh sách tất cả người dùng (Chỉ Admin).
        /// </summary>
        /// <param name="soTrang">Số trang hiện tại.</param>
        /// <param name="kichThuocTrang">Kích thước của mỗi trang.</param>
        /// <returns>Danh sách người dùng có phân trang.</returns>
        [HttpGet("danh-sach")]
        public IActionResult LayDanhSachNguoiDung(int soTrang = 1, int kichThuocTrang = 10)
        {
            // Kiểm tra quyền Admin
            if (!Request.Headers.TryGetValue("X-User-Role", out var vaiTro) || 
                vaiTro.ToString() != "Admin")
            {
                _logger.LogWarning("LayDanhSachNguoiDung: Không có quyền Admin.");
                return StatusCode(403, new { message = "Không có quyền truy cập. Chỉ Admin mới có thể sử dụng chức năng này." });
            }

            _logger.LogInformation("Admin yêu cầu lấy danh sách người dùng - Trang: {SoTrang}, Kích thước: {KichThuoc}", soTrang, kichThuocTrang);

            var tatCaNguoiDung = _mockHoSoDb;
            var nguoiDungPhanTrang = tatCaNguoiDung
                .Skip((soTrang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToList();

            var tongSoMuc = tatCaNguoiDung.Count;
            var tongSoTrang = (int)Math.Ceiling((double)tongSoMuc / kichThuocTrang);

            var ketQua = new
            {
                SoTrangHienTai = soTrang,
                TongSoTrang = tongSoTrang,
                TongSoMuc = tongSoMuc,
                DuLieu = nguoiDungPhanTrang.Select(hoSo => new
                {
                    Id = hoSo.UserId,
                    HoTen = hoSo.HoTen,
                    SoDienThoai = hoSo.SoDienThoai,
                    NgaySinh = hoSo.NgaySinh,
                    DiaChi = hoSo.DiaChi,
                    TaoLuc = hoSo.TaoLuc,
                    CapNhatLuc = hoSo.CapNhatLuc
                })
            };

            return Ok(ketQua);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng theo ID (Chỉ Admin).
        /// </summary>
        /// <param name="id">ID của người dùng.</param>
        /// <returns>Thông tin chi tiết người dùng.</returns>
        [HttpGet("{id}/chi-tiet")]
        public IActionResult LayChiTietNguoiDung(string id)
        {
            // Kiểm tra quyền Admin
            if (!Request.Headers.TryGetValue("X-User-Role", out var vaiTro) || 
                vaiTro.ToString() != "Admin")
            {
                _logger.LogWarning("LayChiTietNguoiDung: Không có quyền Admin.");
                return StatusCode(403, new { message = "Không có quyền truy cập. Chỉ Admin mới có thể sử dụng chức năng này." });
            }

            if (!Guid.TryParse(id, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }

            _logger.LogInformation("Admin yêu cầu lấy chi tiết người dùng ID: {UserId}", userId);

            var hoSo = _mockHoSoDb.FirstOrDefault(p => p.UserId == userId);
            if (hoSo == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var ketQua = new
            {
                Id = hoSo.UserId,
                HoTen = hoSo.HoTen,
                SoDienThoai = hoSo.SoDienThoai,
                NgaySinh = hoSo.NgaySinh,
                DiaChi = hoSo.DiaChi,
                AnhDaiDienUrl = hoSo.AnhDaiDienUrl,
                MoTaBanThan = hoSo.MoTaBanThan,
                TaoLuc = hoSo.TaoLuc,
                CapNhatLuc = hoSo.CapNhatLuc
            };

            return Ok(ketQua);
        }

        /// <summary>
        /// Cập nhật thông tin người dùng theo ID (Chỉ Admin).
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật.</param>
        /// <param name="yeuCau">Dữ liệu cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut("{id}/cap-nhat")]
        public IActionResult CapNhatNguoiDung(string id, [FromBody] YeuCauCapNhatHoSo yeuCau)
        {
            // Kiểm tra quyền Admin
            if (!Request.Headers.TryGetValue("X-User-Role", out var vaiTro) || 
                vaiTro.ToString() != "Admin")
            {
                _logger.LogWarning("CapNhatNguoiDung: Không có quyền Admin.");
                return StatusCode(403, new { message = "Không có quyền truy cập. Chỉ Admin mới có thể sử dụng chức năng này." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Guid.TryParse(id, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }

            _logger.LogInformation("Admin yêu cầu cập nhật người dùng ID: {UserId}", userId);

            var hoSo = _mockHoSoDb.FirstOrDefault(p => p.UserId == userId);
            if (hoSo == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            // Cập nhật dữ liệu
            hoSo.HoTen = yeuCau.HoTen;
            hoSo.SoDienThoai = yeuCau.SoDienThoai;
            hoSo.NgaySinh = yeuCau.NgaySinh;
            hoSo.DiaChi = yeuCau.DiaChi;
            hoSo.AnhDaiDienUrl = yeuCau.AnhDaiDienUrl;
            hoSo.MoTaBanThan = yeuCau.MoTaBanThan;
            hoSo.CapNhatLuc = DateTime.UtcNow;

            _logger.LogInformation("Admin đã cập nhật thành công người dùng ID: {UserId}", userId);

            var phanHoi = new PhanHoiHoSo
            {
                ThanhCong = true,
                ThongBao = "Cập nhật người dùng thành công!",
                DuLieu = new ThongTinNguoiDungDto
                {
                    Id = hoSo.UserId,
                    HoTen = hoSo.HoTen,
                    SoDienThoai = hoSo.SoDienThoai,
                    NgaySinh = hoSo.NgaySinh,
                    DiaChi = hoSo.DiaChi,
                    AnhDaiDienUrl = hoSo.AnhDaiDienUrl,
                    MoTaBanThan = hoSo.MoTaBanThan,
                    CapNhatLuc = hoSo.CapNhatLuc,
                    VaiTro = "Admin",
                    Email = "admin@system.com"
                }
            };

            return Ok(phanHoi);
        }

        /// <summary>
        /// Xóa người dùng theo ID (Chỉ Admin).
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa.</param>
        /// <returns>Kết quả xóa.</returns>
        [HttpDelete("{id}")]
        public IActionResult XoaNguoiDung(string id)
        {
            // Kiểm tra quyền Admin
            if (!Request.Headers.TryGetValue("X-User-Role", out var vaiTro) || 
                vaiTro.ToString() != "Admin")
            {
                _logger.LogWarning("XoaNguoiDung: Không có quyền Admin.");
                return StatusCode(403, new { message = "Không có quyền truy cập. Chỉ Admin mới có thể sử dụng chức năng này." });
            }

            if (!Guid.TryParse(id, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }

            _logger.LogInformation("Admin yêu cầu xóa người dùng ID: {UserId}", userId);

            var hoSo = _mockHoSoDb.FirstOrDefault(p => p.UserId == userId);
            if (hoSo == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            // Xóa người dùng khỏi mock database
            _mockHoSoDb.Remove(hoSo);

            _logger.LogInformation("Admin đã xóa thành công người dùng ID: {UserId}", userId);

            return Ok(new { 
                thanhCong = true, 
                thongBao = "Xóa người dùng thành công!",
                userId = userId
            });
        }
    }
}