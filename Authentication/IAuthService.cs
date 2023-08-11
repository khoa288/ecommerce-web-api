using EcommerceWebApi.Entities;
using static EcommerceWebApi.Authentication.AuthService;

namespace EcommerceWebApi.Authentication
{
    public interface IAuthService
    {
        User CurrentUser { get; }

        AuthResult GetQrCode(out string? uriString);
        Task<AuthResult> Login(string username, string password, HttpContext context);
        Task<AuthResult> LoginSecondFactor(string totp, HttpContext context);
        Task<AuthResult> Register(string username, string password);
        Task<AuthResult> RevokeToken(HttpContext context);
        Task<AuthResult> ValidateQrCode(string qrCode, string totp);
    }
}
