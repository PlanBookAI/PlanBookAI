using AuthService.Models.DTOs;
using AuthService.Models.Entities;
using AuthService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic; // Added for List

namespace AuthService.Services
{
    /// <summary>
    /// Lớp triển khai IDichVuXacThuc, chứa toàn bộ logic về JWT.
    /// </summary>
    public class DichVuXacThuc : IDichVuXacThuc
    {
        private readonly IConfiguration _config;
        private readonly byte[] _key;

        // Mock data cho testing
        private readonly List<NguoiDung> _mockUsers = new List<NguoiDung>
        {
            new NguoiDung
            {
                Id = Guid.NewGuid(),
                Email = "admin@test.com",
                HoTen = "Admin User",
                MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword("Password123"),
                VaiTroId = 1,
                LaHoatDong = true,
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow
            },
            new NguoiDung
            {
                Id = Guid.NewGuid(),
                Email = "teacher@test.com",
                HoTen = "Teacher User",
                MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword("Password123"),
                VaiTroId = 4,
                LaHoatDong = true,
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow
            }
        };

        private readonly List<VaiTro> _mockRoles = new List<VaiTro>
        {
            new VaiTro { Id = 1, Ten = "Admin" },
            new VaiTro { Id = 2, Ten = "Manager" },
            new VaiTro { Id = 3, Ten = "Staff" },
            new VaiTro { Id = 4, Ten = "Teacher" }
        };

        public DichVuXacThuc(IConfiguration config)
        {
            _config = config;
            // Lấy khóa bí mật từ file cấu hình (appsettings.json).
            _key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]);
        }

        public async Task<PhanHoiXacThucDto> TaoToken(string email, string matKhau)
        {
            // Debug: In ra số lượng user trong mock data
            Console.WriteLine($"Debug: Số lượng user trong mock data: {_mockUsers.Count}");
            Console.WriteLine($"Debug: Đang tìm user với email: {email}");
            
            // Tìm người dùng trong mock data theo email
            var nguoiDung = _mockUsers.FirstOrDefault(u => u.Email == email);

            // Debug: In ra kết quả tìm kiếm
            if (nguoiDung == null)
            {
                Console.WriteLine($"Debug: Không tìm thấy user với email: {email}");
                Console.WriteLine($"Debug: Danh sách email có sẵn: {string.Join(", ", _mockUsers.Select(u => u.Email))}");
            }
            else
            {
                Console.WriteLine($"Debug: Tìm thấy user: {nguoiDung.Email}, VaiTroId: {nguoiDung.VaiTroId}");
            }

            // Xử lý lỗi: nếu không tìm thấy người dùng hoặc mật khẩu không đúng.
            if (nguoiDung == null || !BCrypt.Net.BCrypt.Verify(matKhau, nguoiDung.MatKhauMaHoa))
            {
                throw new UnauthorizedAccessException("Thông tin đăng nhập không hợp lệ.");
            }

            // Tạo access token và refresh token.
            var accessToken = TaoAccessToken(nguoiDung);
            var refreshToken = TaoRefreshToken();

            return new PhanHoiXacThucDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };
        }

        public async Task<PhanHoiXacThucDto> DangKy(string email, string matKhau)
        {
            // Kiểm tra email đã tồn tại chưa trong mock data
            var nguoiDungTonTai = _mockUsers.FirstOrDefault(u => u.Email == email);
            if (nguoiDungTonTai != null)
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            // Tạo người dùng mới
            var nguoiDungMoi = new NguoiDung
            {
                Id = Guid.NewGuid(),
                Email = email,
                HoTen = "Người dùng mới",
                MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword(matKhau),
                VaiTroId = 4, // Mặc định là Teacher
                LaHoatDong = true,
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow
            };

            // Thêm vào mock data
            _mockUsers.Add(nguoiDungMoi);
            
            // Debug: In ra thông tin sau khi thêm user
            Console.WriteLine($"Debug: Đã thêm user mới: {nguoiDungMoi.Email}");
            Console.WriteLine($"Debug: Tổng số user trong mock data sau khi thêm: {_mockUsers.Count}");

            // Tạo token cho người dùng mới
            var accessToken = TaoAccessToken(nguoiDungMoi);
            var refreshToken = TaoRefreshToken();

            return new PhanHoiXacThucDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };
        }

        public async Task<PhanHoiXacThucDto> LamMoiToken(string refreshToken)
        {
            // Note: Entity hiện tại không có RefreshToken, nên trả về lỗi
            throw new UnauthorizedAccessException("Chức năng làm mới token chưa được hỗ trợ.");
        }

        public string XacThucToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                // Cấu hình các tham số để xác thực token.
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["JwtSettings:Issuer"],
                ValidAudience = _config["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(_key)
            };

            try
            {
                // Thử xác thực token.
                tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                // Trích xuất email người dùng từ token.
                return jwtToken?.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
            }
            catch
            {
                // Trả về null nếu token không hợp lệ.
                return null;
            }
        }

        /// <summary>
        /// Phương thức nội bộ để tạo access token.
        /// </summary>
        private JwtSecurityToken TaoAccessToken(NguoiDung nguoiDung)
        {
            var vaiTro = _mockRoles.FirstOrDefault(r => r.Id == nguoiDung.VaiTroId);
            
            var claims = new[]
            {
                // Thêm các claim (thông tin) vào token.
                new Claim(ClaimTypes.Name, nguoiDung.Email),
                new Claim(ClaimTypes.Role, vaiTro?.Ten ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // Thời gian hết hạn của token.
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        /// <summary>
        /// Phương thức nội bộ để tạo refresh token ngẫu nhiên.
        /// </summary>
        private string TaoRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
