using LoginJWT.Entities;
using LoginJWT.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LoginJWT.Utils
{
    public class JWTHelper
    {
        private readonly AppSettings _applicationSettings;
        private readonly UserService _userService;

        public JWTHelper(AppSettings applicationSettings, UserService userService)
        {
            _applicationSettings = applicationSettings;
            _userService = userService;
        }

        public void SetRefreshToken(RefreshToken refreshToken, User user, HttpContext context)
        {
            // Set refresh token to cookie
            context.Response.Cookies.Append(
                "refresh_token",
                refreshToken.Token,
                new CookieOptions
                {
                    Expires = refreshToken.Expires,
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                }
            );

            // Set refresh token to user
            _userService.UpdateUser(user, refreshToken: refreshToken);
        }

        public void SetJWT(string encrypterToken, HttpContext context)
        {
            // Set JWT to cookie
            context.Response.Cookies.Append(
                "access_token",
                encrypterToken,
                new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(15),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None
                }
            );
        }

        public dynamic JWTGenerator(User user, bool isSecondFactorChecked, HttpContext context)
        {
            // Create JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_applicationSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim("id", user.UserName),
                        new Claim("status", isSecondFactorChecked.ToString())
                    }
                ),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);

            // Set JWT
            SetJWT(encrypterToken, context);

            // Create & set refresh token
            if (isSecondFactorChecked)
            {
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, user, context);
            }

            return encrypterToken;
        }

        private static RefreshToken GenerateRefreshToken()
        {
            // Create refresh token
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        public void RevokeToken(User user, HttpContext context)
        {
            // Revoke all tokens from cookies and user
            _userService.UpdateUser(user, refreshToken: null);

            context.Response.Cookies.Delete("access_token");
            context.Response.Cookies.Delete("refresh_token");
        }

        public string? GetUsername(HttpContext context)
        {
            // Get user from JWT
            var tokenString = context.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(tokenString))
            {
                return null;
            }

            try
            {
                // Read token
                var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
                return token.Claims.First(x => x.Type == "id").Value;
            }
            catch
            {
                return null;
            }
        }

        public string? GetIsSecondFactorChecked(HttpContext context)
        {
            // Get auth status from JWT
            var tokenString = context.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(tokenString))
            {
                return null;
            }

            try
            {
                // Read token
                var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
                return token.Claims.First(x => x.Type == "status").Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
