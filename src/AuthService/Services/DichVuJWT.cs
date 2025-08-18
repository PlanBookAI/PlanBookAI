using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuthService.Models.Entities;

namespace AuthService.Services;

public class DichVuJWT : IDichVuJWT
{
    private readonly IConfiguration _configuration;

    public DichVuJWT(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string TaoToken(NguoiDung nguoiDung)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
            new(ClaimTypes.Email, nguoiDung.Email),
            new(ClaimTypes.Role, nguoiDung.VaiTro.Ten),
            new("VaiTroId", nguoiDung.VaiTroId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60")
            ),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string TaoRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool XacThucToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Guid LayNguoiDungIdTuToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }

    public string LayEmailTuToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        return emailClaim?.Value ?? string.Empty;
    }

    public int LayVaiTroIdTuToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var vaiTroIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "VaiTroId");
        return vaiTroIdClaim != null ? int.Parse(vaiTroIdClaim.Value) : 0;
    }
}
