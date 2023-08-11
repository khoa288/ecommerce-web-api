using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApi.Entities
{
    public class Order
    {
        [Required(ErrorMessage = "{0} is required")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "{0} is required")]
        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required")]
        public Dictionary<int, int> ProductList { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required")]
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Successed,
        Canceled
    }
}
