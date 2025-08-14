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
        private readonly INguoiDungRepository _nguoiDungRepository;
        private readonly IVaiTroRepository _vaiTroRepository;

        public DichVuXacThuc(IConfiguration config, INguoiDungRepository nguoiDungRepository, IVaiTroRepository vaiTroRepository)
        {
            _config = config;
            _nguoiDungRepository = nguoiDungRepository;
            _vaiTroRepository = vaiTroRepository;
            // Lấy khóa bí mật từ file cấu hình (appsettings.json).
            _key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]);
        }

        public async Task<PhanHoiXacThucDto> TaoToken(string email, string matKhau)
        {
            // Tìm người dùng trong database theo email
            var nguoiDung = await _nguoiDungRepository.GetByEmailAsync(email);

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

        public async Task<PhanHoiXacThucDto> DangKy(string email, string matKhau, string hoTen, string vaiTro)
        {
            // Kiểm tra email đã tồn tại chưa trong database
            var nguoiDungTonTai = await _nguoiDungRepository.GetByEmailAsync(email);
            if (nguoiDungTonTai != null)
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            // Lấy role ID từ tên vai trò
            var vaiTroEntity = await _vaiTroRepository.GetByTenVaiTroAsync(vaiTro);
            if (vaiTroEntity == null)
            {
                throw new InvalidOperationException("Vai trò không hợp lệ.");
            }

            // Tạo người dùng mới
            var nguoiDungMoi = new NguoiDung
            {
                Id = Guid.NewGuid(),
                Email = email,
                HoTen = hoTen,
                MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword(matKhau),
                VaiTroId = vaiTroEntity.Id,
                LaHoatDong = true,
                TaoLuc = DateTime.UtcNow,
                CapNhatLuc = DateTime.UtcNow
            };

            // Lưu vào database
            await _nguoiDungRepository.AddAsync(nguoiDungMoi);

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
            var claims = new[]
            {
                // Thêm các claim (thông tin) vào token.
                new Claim(ClaimTypes.Name, nguoiDung.Email),
                new Claim(ClaimTypes.Role, nguoiDung.VaiTroId.ToString()),
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
