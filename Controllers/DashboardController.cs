using LoginForm.Services;
using LoginForm.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LoginForm.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilter))]
    [Controller]
    public class DashboardController : Controller
    {
        private readonly AppSettings _applicationSettings;
        private readonly UserService _userService;
        private readonly JWTHelper _jwt;

        public DashboardController(
            IOptions<AppSettings> applicationSettings,
            UserService userService
        )
        {
            _applicationSettings = applicationSettings.Value;
            _userService = userService;
            _jwt = new JWTHelper(_applicationSettings, _userService);
        }

        [AllowAnonymous]
        [HttpGet("GetTime")]
        public IActionResult GetTime()
        {
            try
            {
                return Ok(DateTime.Now.ToString());
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowFirstFactor]
        [HttpGet("GetUserName")]
        public IActionResult GetUserName()
        {
            try
            {
                var username = _jwt.GetUsername(HttpContext);
                return Ok(username);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
