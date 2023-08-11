using EcommerceWebApi.Entities;
using JsonFlatFileDataStore;

namespace EcommerceWebApi.Repositories
{
    public interface IProductRepository
    {
        Task<bool> DeleteAsync(int id);
        IDocumentCollection<Product> GetAll();
        Product? GetById(int id);
        Task<bool> InsertAsync(Product product);
        Task<bool> UpdateAsync(Product product);
    }
}
