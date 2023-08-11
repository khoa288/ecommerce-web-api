using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApi.DTOs
{
    public class ProductDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required")]
        public float Price { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Brand { get; set; } = null!;

        public string Category { get; set; } = null!;

        public Uri Thumbnail { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required")]
        public int Quantity { get; set; }
    }
}
