using System.ComponentModel.DataAnnotations;

namespace LoginForm.Models.Request
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
