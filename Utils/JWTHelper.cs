using LoginJWT.Models;
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
        private readonly HttpClient _httpClient;
        public JWTHelper(AppSettings applicationSettings, HttpClient httpClient)
        {
            this._applicationSettings = applicationSettings;
            this._userService = new UserService();
            this._httpClient = httpClient;
        }
        public void SetRefreshToken(RefreshToken refreshToken, User user, HttpContext context)
        {
            context.Response.Cookies.Append("refresh_token", refreshToken.Token,
                 new CookieOptions
                 {
                     Expires = refreshToken.Expires,
                     HttpOnly = true,
                     Secure = true,
                     IsEssential = true,
                     SameSite = SameSiteMode.None
                 });
            _userService.UpdateUser(user,
                token: refreshToken.Token,
                created: refreshToken.Created,
                expires: refreshToken.Expires);
        }

        public void SetJWT(string encrypterToken, HttpContext context)
        {
            context.Response.Cookies.Append("access_token", encrypterToken,
                  new CookieOptions
                  {
                      Expires = DateTime.Now.AddMinutes(15),
                      HttpOnly = true,
                      Secure = true,
                      IsEssential = true,
                      SameSite = SameSiteMode.None
                  });
        }

        public dynamic JWTGenerator(User user, HttpContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_applicationSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserName) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);

            SetJWT(encrypterToken, context);

            var refreshToken = GenerateRefreshToken();

            SetRefreshToken(refreshToken, user, context);

            return new { token = encrypterToken, username = user.UserName };
        }


        public RefreshTokenResponse RefreshToken(HttpContext context)
        {
            var result = new RefreshTokenResponse()
            {
                ResultCode = RefreshTokenResult.Other
            };
            try
            {
                var refreshToken = context.Request.Cookies["refresh_token"];
                var user = _userService.GetUser(token: refreshToken);
                result.User = user;

                if (user == null)
                {
                    result.ResultCode = RefreshTokenResult.UserNotExist;
                    return result;
                }

                if (user.TokenExpires < DateTime.Now)
                {
                    result.ResultCode = RefreshTokenResult.TokenExpire;
                    return result;
                }

                JWTGenerator(user, context);
                result.ResultCode = RefreshTokenResult.Success;
                return result;
            }
            catch
            {
                return result;      
            }
        }

        public int RevokeToken(string username, HttpContext context)
        {
            try
            {
                _userService.UpdateUser(
                  user: _userService.GetUser(username: username),
                  token: ""
                  );

                context.Response.Cookies.Delete("access_token");
                context.Response.Cookies.Delete("refresh_token");

                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public string? GetUsername(HttpContext context)
        {
            var tokenString = context.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(tokenString))
            {
                return null;
            }

            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);

            return token.Claims.First(x => x.Type == "id").Value;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }
    }
}
