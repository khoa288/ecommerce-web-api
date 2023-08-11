using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;
using static EcommerceWebApi.Services.OrderService;

namespace EcommerceWebApi.Services
{
    public interface IOrderService
    {
        Task<OrderResult> DeleteOrderAsync(string id);
        void Dispose();
        List<Order> GetAllOrders();
        Order? GetOrderById(string id);
        List<Order> GetPaginationOrders(
            PaginationFilter paginationFilter,
            QueryFilter queryFilter,
            out int queryOrderCount
        );
        Task<OrderResult> InsertOrderAsync(string userId, Dictionary<int, int> productList);
        Task<OrderResult> UpdateOrderAsync(Order order);
        Task<OrderResult> UpdateOrderPropertyAsync<T>(Order order, string property, T value);
        Task<OrderResult> UpdateOrderStatusAsync(Order order, OrderStatus status);
    }
}
