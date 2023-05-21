using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Entities
{
    public class User
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public byte[] PasswordSalt { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        public bool IsTwoFactorAuthActivated { get; set; }

        public string? SecretCode { get; set; }
        public string? Token { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
