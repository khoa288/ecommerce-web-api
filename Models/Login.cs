using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public Login(string username, string password)
        {
            UserName = username;
            Password = password;
        }
    }
}
