using AuthService.Models.DTOs;
using AuthService.Models.Entities;
using AuthService.Repositories.NguoiDungRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Services.DichVuXacThuc
{
    /// <summary>
    /// Lớp triển khai IDichVuXacThuc, chứa toàn bộ logic về JWT.
    /// </summary>
    public class DichVuXacThuc : IDichVuXacThuc
    {
        private readonly INguoiDungRepository _nguoiDungRepository;
        private readonly IConfiguration _config;
        private readonly byte[] _key;

        public DichVuXacThuc(INguoiDungRepository nguoiDungRepository, IConfiguration config)
        {
            _nguoiDungRepository = nguoiDungRepository;
            _config = config;
            // Lấy khóa bí mật từ file cấu hình (appsettings.json).
            _key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
        }

        public async Task<PhanHoiXacThucDto> TaoToken(string tenDangNhap, string matKhau)
        {
            // Tìm người dùng trong database.
            var nguoiDung = await _nguoiDungRepository.GetByTenDangNhapAsync(tenDangNhap);

            // Xử lý lỗi: nếu không tìm thấy người dùng hoặc mật khẩu không đúng.
            if (nguoiDung == null || !BCrypt.Net.BCrypt.Verify(matKhau, nguoiDung.MatKhauHash))
            {
                throw new UnauthorizedAccessException("Thông tin đăng nhập không hợp lệ.");
            }

            // Tạo access token và refresh token.
            var accessToken = TaoAccessToken(nguoiDung);
            var refreshToken = TaoRefreshToken();

            // Lưu refresh token vào database.
            nguoiDung.RefreshToken = refreshToken;
            nguoiDung.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _nguoiDungRepository.UpdateAsync(nguoiDung);

            return new PhanHoiXacThucDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };
        }

        public async Task<PhanHoiXacThucDto> LamMoiToken(string refreshToken)
        {
            // Tìm người dùng có refresh token tương ứng.
            var nguoiDung = await _nguoiDungRepository.GetByRefreshTokenAsync(refreshToken);

            // Xử lý lỗi: nếu token không tồn tại hoặc đã hết hạn.
            if (nguoiDung == null || nguoiDung.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Token làm mới không hợp lệ hoặc đã hết hạn.");
            }

            // Tạo token truy cập và refresh token mới.
            var newAccessToken = TaoAccessToken(nguoiDung);
            var newRefreshToken = TaoRefreshToken();

            // Cập nhật refresh token mới vào database.
            nguoiDung.RefreshToken = newRefreshToken;
            nguoiDung.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _nguoiDungRepository.UpdateAsync(nguoiDung);

            return new PhanHoiXacThucDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            };
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
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(_key)
            };

            try
            {
                // Thử xác thực token.
                tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                // Trích xuất tên người dùng từ token.
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
            var claims = new[]
            {
                // Thêm các claim (thông tin) vào token.
                new Claim(ClaimTypes.Name, nguoiDung.TenDangNhap),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTro?.TenVaiTro ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
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