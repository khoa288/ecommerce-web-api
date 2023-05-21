using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class RefreshToken
    {
        [Required]
        public string Token { get; set; }

        public RefreshToken(string token)
        {
            Token = token;
        }

        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
