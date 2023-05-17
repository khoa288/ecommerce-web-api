using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class TwoFactorAuth
    {
        public string? UserName { get; set;}
        [Required]
        public string Totp { get; set; }
    }
}
