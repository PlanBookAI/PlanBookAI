using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using System.Security.Claims;
using UserService.Repositories;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/v1/nguoi-dung")]
    public class NguoiDungController : ControllerBase
    {
        private readonly ILogger<NguoiDungController> _logger;

        // Sử dụng database thực
        private readonly IHoSoNguoiDungRepository _hoSoRepository;

        public NguoiDungController(ILogger<NguoiDungController> logger, IHoSoNguoiDungRepository hoSoRepository)
        {
            _logger = logger;
            _hoSoRepository = hoSoRepository;
        }

        /// <summary>
        /// Lấy thông tin hồ sơ của người dùng đang đăng nhập.
        /// </summary>
        /// <returns>Thông tin chi tiết hồ sơ người dùng.</returns>
        [HttpGet("thong-tin")]
        public async Task<IActionResult> GetThongTin()
        {
            // Lấy thông tin từ User.Identity đã được set bởi HeaderAuthenticationMiddleware
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("GetThongTin: User không được xác thực.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdStr))
            {
                _logger.LogWarning("GetThongTin: Không thể lấy User ID từ claims.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }
            
            // Tìm user trong database bằng email
            var user = await _hoSoRepository.GetByEmailAsync(userIdStr);
            if (user == null)
            {
                _logger.LogWarning("GetThongTin: Không tìm thấy user với email: {Email}", userIdStr);
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }
            
            var userId = user.UserId;

            var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value ?? "N/A";
            var email = User.FindFirst(ClaimTypes.Name)?.Value ?? "N/A";

            _logger.LogInformation("Nhận yêu cầu lấy thông tin cho User ID: {UserId}", userId);

            var hoSo = await _hoSoRepository.GetByIdAsync(userId.ToString());

            if (hoSo == null)
            {
                // Tạo profile mới cho admin user
                hoSo = new HoSoNguoiDung
                {
                    UserId = userId,
                    HoTen = "Admin User",
                    SoDienThoai = "0123456789",
                    DiaChi = "Admin Address",
                    NgaySinh = DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc),
                    AnhDaiDienUrl = "https://example.com/admin-avatar.jpg",
                    MoTaBanThan = "Administrator của hệ thống PlanbookAI.",
                    TaoLuc = DateTime.UtcNow,
                    CapNhatLuc = DateTime.UtcNow
                };
                
                await _hoSoRepository.AddAsync(hoSo);
                _logger.LogInformation("Đã tạo profile mới cho admin user: {UserId}", userId);
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
        public async Task<IActionResult> CapNhatHoSo([FromBody] YeuCauCapNhatHoSo yeuCau)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy thông tin từ User.Identity đã được set bởi HeaderAuthenticationMiddleware
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("CapNhatHoSo: User không được xác thực.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("CapNhatHoSo: Không thể lấy User Email từ claims.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            _logger.LogInformation("Nhận yêu cầu cập nhật hồ sơ cho User Email: {UserEmail}", userEmail);

            // Tìm user trong database bằng email
            var user = await _hoSoRepository.GetByEmailAsync(userEmail);
            if (user == null)
            {
                _logger.LogWarning("CapNhatHoSo: Không tìm thấy user với email: {UserEmail}", userEmail);
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            var userId = user.UserId;
            var hoSo = await _hoSoRepository.GetByIdAsync(userId.ToString());
            if (hoSo == null)
            {
                _logger.LogWarning("Không tìm thấy hồ sơ để cập nhật cho User ID: {UserId}", userId);
                return NotFound(new { message = "Không tìm thấy hồ sơ người dùng." });
            }

            // Cập nhật dữ liệu từ DTO vào entity
            hoSo.HoTen = yeuCau.HoTen;
            hoSo.SoDienThoai = yeuCau.SoDienThoai;
            hoSo.NgaySinh = yeuCau.NgaySinh.HasValue ? 
                (yeuCau.NgaySinh.Value.Kind == DateTimeKind.Unspecified ? 
                    DateTime.SpecifyKind(yeuCau.NgaySinh.Value, DateTimeKind.Utc) : 
                    yeuCau.NgaySinh.Value.ToUniversalTime()) : 
                hoSo.NgaySinh;
            hoSo.DiaChi = yeuCau.DiaChi;
            hoSo.AnhDaiDienUrl = yeuCau.AnhDaiDienUrl;
            hoSo.MoTaBanThan = yeuCau.MoTaBanThan;
            hoSo.CapNhatLuc = DateTime.UtcNow;

            // Lưu vào database
            await _hoSoRepository.UpdateAsync(hoSo);

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
                    VaiTro = User.FindFirst(ClaimTypes.Role)?.Value ?? "N/A",
                    Email = User.FindFirst(ClaimTypes.Name)?.Value ?? "N/A"
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
        public async Task<IActionResult> LayDanhSachNguoiDung(int soTrang = 1, int kichThuocTrang = 10)
        {
            // Kiểm tra quyền Admin từ User.Identity
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("LayDanhSachNguoiDung: User không được xác thực.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value;
            if (vaiTro != "1")
            {
                _logger.LogWarning("LayDanhSachNguoiDung: Không có quyền Admin.");
                return StatusCode(403, new { message = "Không có quyền truy cập. Chỉ Admin mới có thể sử dụng chức năng này." });
            }

            _logger.LogInformation("Admin yêu cầu lấy danh sách người dùng - Trang: {SoTrang}, Kích thước: {KichThuoc}", soTrang, kichThuocTrang);

            var tatCaNguoiDung = await _hoSoRepository.GetAllAsync();
            var nguoiDungPhanTrang = tatCaNguoiDung
                .Skip((soTrang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToList();

            var tongSoMuc = tatCaNguoiDung.Count();
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
        public async Task<IActionResult> LayChiTietNguoiDung(string id)
        {
            // Kiểm tra quyền Admin từ User.Identity
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("LayChiTietNguoiDung: User không được xác thực.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value;
            if (vaiTro != "1")
            {
                _logger.LogWarning("LayChiTietNguoiDung: Không có quyền Admin.");
                return StatusCode(403, new { message = "Không có quyền truy cập. Chỉ Admin mới có thể sử dụng chức năng này." });
            }

            if (!Guid.TryParse(id, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }

            _logger.LogInformation("Admin yêu cầu lấy chi tiết người dùng ID: {UserId}", userId);

            var hoSo = await _hoSoRepository.GetByIdAsync(id);
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
        public async Task<IActionResult> CapNhatNguoiDung(string id, [FromBody] YeuCauCapNhatHoSo yeuCau)
        {
            // Kiểm tra quyền Admin từ User.Identity
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("CapNhatNguoiDung: User không được xác thực.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value;
            if (vaiTro != "1")
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

            var hoSo = await _hoSoRepository.GetByIdAsync(id);
            if (hoSo == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            // Cập nhật dữ liệu
            hoSo.HoTen = yeuCau.HoTen;
            hoSo.SoDienThoai = yeuCau.SoDienThoai;
            hoSo.NgaySinh = yeuCau.NgaySinh.HasValue ? 
                (yeuCau.NgaySinh.Value.Kind == DateTimeKind.Unspecified ? 
                    DateTime.SpecifyKind(yeuCau.NgaySinh.Value, DateTimeKind.Utc) : 
                    yeuCau.NgaySinh.Value.ToUniversalTime()) : 
                hoSo.NgaySinh;
            hoSo.DiaChi = yeuCau.DiaChi;
            hoSo.AnhDaiDienUrl = yeuCau.AnhDaiDienUrl;
            hoSo.MoTaBanThan = yeuCau.MoTaBanThan;
            hoSo.CapNhatLuc = DateTime.UtcNow;

            // Lưu vào database
            await _hoSoRepository.UpdateAsync(hoSo);

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
                    VaiTro = vaiTro ?? "N/A",
                    Email = User.FindFirst(ClaimTypes.Name)?.Value ?? "N/A"
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
        public async Task<IActionResult> XoaNguoiDung(string id)
        {
            // Kiểm tra quyền Admin từ User.Identity
            if (User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("XoaNguoiDung: User không được xác thực.");
                return Unauthorized(new { message = "Thông tin xác thực không hợp lệ." });
            }

            var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value;
            if (vaiTro != "1")
            {
                _logger.LogWarning("XoaNguoiDung: Không có quyền Admin.");
                return StatusCode(403, new { message = "Không có quyền truy cập. Chỉ Admin mới có thể sử dụng chức năng này." });
            }

            if (!Guid.TryParse(id, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }

            _logger.LogInformation("Admin yêu cầu xóa người dùng ID: {UserId}", userId);

            var hoSo = await _hoSoRepository.GetByIdAsync(id);
            if (hoSo == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            // Xóa người dùng khỏi database
            await _hoSoRepository.DeleteAsync(id);

            _logger.LogInformation("Admin đã xóa thành công người dùng ID: {UserId}", userId);

            return Ok(new { 
                thanhCong = true, 
                thongBao = "Xóa người dùng thành công!",
                userId = userId
            });
        }
    }
}