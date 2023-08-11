using EcommerceWebApi.Authentication;
using EcommerceWebApi.Entities;
using EcommerceWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceWebApi.Filters
{
    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthorizeFilter> _logger;

        public AuthorizeFilter(
            AuthService authService,
            UserService userService,
            JwtService jwtService,
            ILogger<AuthorizeFilter> logger
        )
        {
            _authService = authService;
            _userService = userService;
            _jwtService = jwtService;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                    .OfType<AllowAnonymousAttribute>()
                    .Any();
                if (allowAnonymous)
                {
                    return;
                }

                var allowFirstFactor = context.ActionDescriptor.EndpointMetadata
                    .OfType<AllowFirstFactorAttribute>()
                    .Any();

                var requiredRoles = context.ActionDescriptor.EndpointMetadata
                    .OfType<AuthorizeRoleAttribute>()
                    .Select(a => a.Role);

                var accessTokenString = context.HttpContext.Request.Cookies["access_token"];
                var refreshTokenString = context.HttpContext.Request.Cookies["refresh_token"];

                if (!allowFirstFactor)
                {
                    if (!CheckRefreshToken(refreshTokenString, context, out User? user))
                    {
                        return;
                    }
                    ;

                    if (string.IsNullOrEmpty(accessTokenString) && user != null)
                    {
                        _ = _jwtService.GenerateJWT(
                            user,
                            isSecondFactorChecked: true,
                            context.HttpContext
                        );

                        return;
                    }
                    else
                    {
                        if (
                            !CheckAccessToken(
                                accessTokenString,
                                user,
                                context,
                                out JwtSecurityToken accessToken,
                                out User currentUser
                            )
                        )
                        {
                            return;
                        }

                        if (requiredRoles.Any() && !CheckRole(accessToken, requiredRoles, context))
                        {
                            return;
                        }
                        ;

                        _authService.CurrentUser = currentUser;
                    }
                }
                else
                {
                    if (
                        !CheckAccessToken(
                            accessTokenString,
                            null,
                            context,
                            out JwtSecurityToken accessToken,
                            out User currentUser
                        )
                    )
                    {
                        return;
                    }
                    ;
                    if (requiredRoles.Any() && !CheckRole(accessToken, requiredRoles, context))
                    {
                        return;
                    }
                    ;

                    _authService.CurrentUser = currentUser;

                    context.Result = new ContentResult()
                    {
                        Content = "Need second factor authentication",
                        StatusCode = StatusCodes.Status200OK
                    };
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }
        }

        private bool CheckRefreshToken(
            string? refreshTokenString,
            AuthorizationFilterContext context,
            out User? user
        )
        {
            user = null!;
            try
            {
                user = null;
                if (string.IsNullOrEmpty(refreshTokenString))
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return false;
                }

                user = _userService.GetUserByToken(token: refreshTokenString);

                if (user == null || user.RefreshToken.Expires < DateTime.Now)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Unauthorized",
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return false;
            }
        }

        private bool CheckAccessToken(
            string? accessTokenString,
            User? user,
            AuthorizationFilterContext context,
            out JwtSecurityToken accessToken,
            out User currentUser
        )
        {
            accessToken = null!;
            try
            {
                accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessTokenString);
                var accessTokenId = accessToken.Claims.FirstOrDefault(x => x.Type == "id");
                var isSecondFactorChecked = accessToken.Claims.FirstOrDefault(
                    x => x.Type == "status"
                );

                if (accessTokenId == null || isSecondFactorChecked == null)
                {
                    currentUser = null!;
                    return false;
                }

                currentUser = _userService.GetUserById(accessTokenId.Value)!;

                if (
                    (
                        user != null
                        && accessTokenId.Value == user.Id
                        && isSecondFactorChecked.Value == "True"
                    ) || (user == null && currentUser != null)
                )
                {
                    return true;
                }
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                currentUser = null!;
                context.Result = new ContentResult()
                {
                    Content = "Unauthorized",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return false;
            }
        }

        private bool CheckRole(
            JwtSecurityToken accessToken,
            IEnumerable<string>? requiredRoles,
            AuthorizationFilterContext context
        )
        {
            try
            {
                if (requiredRoles == null)
                {
                    return false;
                }

                if (requiredRoles.Any())
                {
                    var userRoleClaim = accessToken.Claims.FirstOrDefault(c => c.Type == "role");
                    if (userRoleClaim == null || !requiredRoles.Contains(userRoleClaim.Value))
                    {
                        context.Result = new ContentResult()
                        {
                            Content = "Forbidden",
                            StatusCode = StatusCodes.Status403Forbidden
                        };
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message} - {ex.InnerException?.Message}");
                context.Result = new ContentResult()
                {
                    Content = "Forbidden",
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowFirstFactorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute
    {
        public string Role { get; }

        public AuthorizeRoleAttribute(string role)
        {
            Role = role;
        }
    }
}
