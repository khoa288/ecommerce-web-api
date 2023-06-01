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
            try
            {
                // Check if allow anonymous
                var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                    .OfType<AllowAnonymousAttribute>()
                    .Any();
                if (allowAnonymous)
                {
                    return;
                }

                // Get access_token
                var accessTokenString = context.HttpContext.Request.Cookies["access_token"];

                // If access_token is empty
                if (string.IsNullOrEmpty(accessTokenString))
                {
                    // Get refresh_token
                    var refreshTokenString = context.HttpContext.Request.Cookies["refresh_token"];

                    // If refresh_token is empty
                    if (string.IsNullOrEmpty(refreshTokenString))
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "Unauthorized",
                            StatusCode = StatusCodes.Status401Unauthorized
                        };
                        return;
                    }

                    // Check refresh_token
                    var user = _userService.GetUser(token: refreshTokenString);
                    if (user == null || user.RefreshToken.Expires < DateTime.Now)
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "Unauthorized",
                            StatusCode = StatusCodes.Status401Unauthorized
                        };
                        return;
                    }
                    else
                    {
                        _jwt.JWTGenerator(user, isSecondFactorChecked: true, context.HttpContext);
                        return;
                    }
                }

                // Check access_token
                var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessTokenString);
                var accessTokenId = accessToken.Claims.FirstOrDefault(x => x.Type == "id");

                if (
                    accessTokenId == null
                    || _userService.GetUser(username: accessTokenId.Value) == null
                )
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return;
                }

                // Check if allow only first factor checked
                var allowFirstFactor = context.ActionDescriptor.EndpointMetadata
                    .OfType<AllowFirstFactorAttribute>()
                    .Any();
                if (!allowFirstFactor)
                {
                    // Check if second factor checked
                    var isSecondFactorChecked = _jwt.GetIsSecondFactorChecked(context.HttpContext);
                    if (isSecondFactorChecked != "True")
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "Unauthorized",
                            StatusCode = StatusCodes.Status200OK
                        };
                        return;
                    }
                }
            }
            catch
            {
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowFirstFactorAttribute : Attribute { }
}
