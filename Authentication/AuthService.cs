using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;
using System.Security.Cryptography;

namespace EcommerceWebApi.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITotpService _totpService;
        private readonly IJwtService _jwtService;
        public User CurrentUser { get; internal set; } = null!;

        public AuthService(UserService userService, TotpService totpService, JwtService jwtService)
        {
            _userService = userService;
            _totpService = totpService;
            _jwtService = jwtService;
        }

        public enum AuthResult
        {
            UserNotExist,
            UserExisted,
            InvalidCredentials,
            NeedSecondFactorAuth,
            Success,
            Fail
        }

        private static bool CheckPassword(User user, string password)
        {
            using HMACSHA512? hmac = new(key: user.PasswordSalt);
            var compute = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return compute.SequenceEqual(user.PasswordHash);
        }

        public async Task<AuthResult> Login(string username, string password, HttpContext context)
        {
            var user = _userService.GetUserByName(username);
            if (user == null)
            {
                return AuthResult.UserNotExist;
            }

            if (!CheckPassword(user, password))
            {
                return AuthResult.InvalidCredentials;
            }

            if (user.IsTwoFactorAuthActivated)
            {
                return await _jwtService.GenerateJWT(user, isSecondFactorChecked: false, context)
                    ? AuthResult.NeedSecondFactorAuth
                    : AuthResult.Fail;
            }
            return await _jwtService.GenerateJWT(user, isSecondFactorChecked: true, context)
                ? AuthResult.Success
                : AuthResult.Fail;
        }

        public async Task<AuthResult> LoginSecondFactor(string totp, HttpContext context)
        {
            bool validated = _totpService.ValidateTotp(
                base32Secret: CurrentUser.SecretCode,
                totp: totp
            );
            if (!validated)
            {
                return AuthResult.InvalidCredentials;
            }

            var result = await _jwtService.RevokeToken(CurrentUser, context);
            if (!result)
            {
                return AuthResult.Fail;
            }

            return await _jwtService.GenerateJWT(CurrentUser, isSecondFactorChecked: true, context)
                ? AuthResult.Success
                : AuthResult.Fail;
        }

        public async Task<AuthResult> Register(string username, string password)
        {
            if (_userService.GetUserByName(username) != null)
            {
                return AuthResult.UserExisted;
            }

            var user = new User { Username = username };
            using (HMACSHA512? hmac = new())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                user.Role = "User";
                user.IsTwoFactorAuthActivated = false;
            }
            ;

            var insertResult = await _userService.InsertUserAsync(user);
            return insertResult ? AuthResult.Success : AuthResult.Fail;
        }

        public async Task<AuthResult> RevokeToken(HttpContext context)
        {
            var revokeResult = await _jwtService.RevokeToken(CurrentUser, context);

            if (revokeResult)
            {
                return AuthResult.Success;
            }
            else
            {
                return AuthResult.Fail;
            }
        }

        public AuthResult GetQrCode(out string? uriString)
        {
            uriString = _totpService.GenerateQrCode(
                base32Secret: _totpService.GenerateBase32Secret(),
                username: CurrentUser.Username
            );

            return AuthResult.Success;
        }

        public async Task<AuthResult> ValidateQrCode(string qrCode, string totp)
        {
            string? secret = _totpService.GetSecretFromQrCode(qrCode);
            if (secret == null)
            {
                return AuthResult.InvalidCredentials;
            }

            bool validated = _totpService.ValidateTotp(base32Secret: secret, totp: totp);
            if (!validated)
            {
                return AuthResult.InvalidCredentials;
            }

            CurrentUser.SecretCode = secret;
            var updateResult = await _userService.UpdateUserAsync(CurrentUser);

            return updateResult ? AuthResult.Success : AuthResult.Fail;
        }
    }
}
