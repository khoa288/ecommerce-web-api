using EcommerceWebApi.Entities;
using JsonFlatFileDataStore;

namespace EcommerceWebApi.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> DeleteAsync(string id);
        IDocumentCollection<Order> GetAll();
        Order? GetById(string id);
        Task<bool> InsertAsync(Order order);
        Task<bool> UpdateAsync(Order order);
    }
}
