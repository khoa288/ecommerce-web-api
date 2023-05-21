using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class User
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public bool IsFirstFactorChecked { get; set; }
        [Required]
        public bool IsTwoFactorAuthActivated { get; set; }

        public string? SecretCode { get; set; }
        public string? Token { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
