using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Entities
{
    public class RefreshToken
    {
        [Required]
        public string Token { get; set; } = null!;

        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
