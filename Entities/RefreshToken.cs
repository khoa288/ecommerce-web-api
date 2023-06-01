using System.ComponentModel.DataAnnotations;

namespace LoginForm.Entities
{
    public class RefreshToken
    {
        [Required]
        public string Token { get; set; } = null!;

        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
