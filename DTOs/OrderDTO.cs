using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApi.DTOs
{
    public class OrderDTO
    {
        [Required]
        public Dictionary<int, int> ProductList { get; set; } = null!;
    }
}
