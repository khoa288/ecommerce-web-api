using EcommerceWebApi.Entities;

namespace EcommerceWebApi.Authentication
{
    public interface IJwtService
    {
        Task<bool> GenerateJWT(User user, bool isSecondFactorChecked, HttpContext context);
        string? GetTotpStatusFromJWT(HttpContext context);
        User? GetUserFromJWT(HttpContext context);
        Task<bool> RevokeToken(User user, HttpContext context);
        void SetJWT(string encrypterToken, HttpContext context);
        Task<bool> SetRefreshToken(RefreshToken refreshToken, User user, HttpContext context);
    }
}
