using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class Register
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

        public Register(string userName, string password, string confirmPassword)
        {
            Username = userName;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
}
