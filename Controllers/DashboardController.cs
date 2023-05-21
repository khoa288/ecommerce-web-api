using LoginJWT.Services;
using LoginJWT.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoginJWT.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class DashboardController : Controller
    {
        private readonly AppSettings _applicationSettings;
        private readonly UserService _userService;
        private readonly JWTHelper _jwt;
        public DashboardController(IOptions<AppSettings> applicationSettings, UserService userService)
        {
            _applicationSettings = applicationSettings.Value;
            _userService = userService;
            _jwt = new JWTHelper(_applicationSettings, _userService);
        }

        [HttpGet("GetTime")]
        public string GetTime()
        {
            return DateTime.Now.ToString();
        }

        [HttpGet("GetUserName")]
        public IActionResult GetUserName()
        {
            var username = _jwt.GetUsername(HttpContext);
            return Ok(username);
        }
    }
}
