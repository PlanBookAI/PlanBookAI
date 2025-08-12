using GatewayService.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GatewayService.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public Task<string?> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenValidationParameters = GetTokenValidationParameters();
                var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var userId = principal.FindFirst("userId")?.Value 
                               ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? principal.FindFirst("sub")?.Value
                               ?? principal.FindFirst(ClaimTypes.Name)?.Value; // Thêm claim Name từ AuthService

                    _logger.LogInformation("Token validation successful for user: {UserId}", userId);
                    return Task.FromResult(userId);
                }

                return Task.FromResult<string?>(null);
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("Token expired: {Message}", ex.Message);
                return Task.FromResult<string?>(null);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token validation failed: {Message}", ex.Message);
                return Task.FromResult<string?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                return Task.FromResult<string?>(null);
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var jwtToken = _tokenHandler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true; // Nếu không đọc được token thì coi như expired
            }
        }

        public Task<Dictionary<string, string>?> GetUserClaimsAsync(string token)
        {
            try
            {
                var jwtToken = _tokenHandler.ReadJwtToken(token);
                var claims = new Dictionary<string, string>();

                foreach (var claim in jwtToken.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }

                return Task.FromResult<Dictionary<string, string>?>(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user claims from token");
                return Task.FromResult<Dictionary<string, string>?>(null);
            }
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Không cho phép thời gian lệch
            };
        }
    }
}
