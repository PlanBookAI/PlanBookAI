using AuthService.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class DichVuJWT : IDichVuJWT
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DichVuJWT> _logger;

        public DichVuJWT(IConfiguration configuration, ILogger<DichVuJWT> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string TaoAccessToken(NguoiDung nguoiDung, VaiTro vaiTro)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
                var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
                var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
                var expiryHours = int.Parse(jwtSettings["ExpiryInHours"] ?? "24");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
                    new Claim("userId", nguoiDung.Id.ToString()),
                    new Claim(ClaimTypes.Email, nguoiDung.Email),
                    new Claim(ClaimTypes.Name, nguoiDung.HoTen),
                    new Claim(ClaimTypes.Role, vaiTro.Ten),
                    new Claim("roleId", vaiTro.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(expiryHours),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("Access token created for user {UserId}", nguoiDung.Id);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating access token for user {UserId}", nguoiDung.Id);
                throw;
            }
        }

        public string TaoRefreshToken(Guid nguoiDungId, bool ghiNho = false)
        {
            try
            {
                var randomBytes = new byte[32];
                using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);

                var refreshToken = Convert.ToBase64String(randomBytes);

                _logger.LogInformation("Refresh token created for user {UserId}, RememberMe: {RememberMe}", nguoiDungId, ghiNho);
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating refresh token for user {UserId}", nguoiDungId);
                throw;
            }
        }

        public Guid? ValidateToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
                var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
                var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var userIdClaim = principal.FindFirst("userId")?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogDebug("Token validation successful for user {UserId}", userId);
                    return userId;
                }

                _logger.LogWarning("Invalid user ID in token");
                return null;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token validation failed: Token expired");
                return null;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                return null;
            }
        }

        public Guid? LayUserIdTuToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value
                               ?? jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user ID from token");
                return null;
            }
        }

        public bool KiemTraTokenHetHan(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);

                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token expiration");
                return true; // Assume expired if can't read
            }
        }

        public DateTime? LayThoiGianHetHan(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);

                return jsonToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting token expiration time");
                return null;
            }
        }
    }
}
