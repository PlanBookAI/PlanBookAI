using AuthService.Models.Entities;

namespace AuthService.Services;

public interface IDichVuJWT
{
    string TaoToken(NguoiDung nguoiDung);
    string TaoRefreshToken();
    bool XacThucToken(string token);
    Guid LayNguoiDungIdTuToken(string token);
    string LayEmailTuToken(string token);
    int LayVaiTroIdTuToken(string token);
}
