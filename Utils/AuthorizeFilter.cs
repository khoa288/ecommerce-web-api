using LoginJWT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace LoginJWT.Utils
{
    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly AppSettings _applicationSettings;
        private readonly UserService _userService;
        private readonly JWTHelper _jwt;

        public AuthorizeFilter(IOptions<AppSettings> applicationSettings, UserService userService)
        {
            _applicationSettings = applicationSettings.Value;
            _userService = userService;
            _jwt = new JWTHelper(_applicationSettings, _userService);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var accessTokenString = context.HttpContext.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessTokenString))
            {
                var refreshTokenString = context.HttpContext.Request.Cookies["refresh_token"];
                if (string.IsNullOrEmpty(refreshTokenString))
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    return;
                }

                var user = _userService.GetUser(token: refreshTokenString);
                if (user == null || user.TokenExpires < DateTime.Now)
                {
                    context.Result = new JsonResult(new { message = "Invalid Token" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    return;
                }
                else
                {
                    var generateToken = _jwt.JWTGenerator(user, context.HttpContext);
                    accessTokenString = generateToken.token;
                }
            }

            try
            {
                var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessTokenString);
                var accessTokenCheck = accessToken.Claims.FirstOrDefault(x => x.Type == "id");
                if (accessTokenCheck == null || _userService.GetUser(username: accessTokenCheck.Value) == null)
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    return;
                }
            }
            catch
            {
                context.Result = new JsonResult(new { message = "Invalid Token" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }
        }
    }
}