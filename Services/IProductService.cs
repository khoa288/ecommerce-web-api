using EcommerceWebApi.Entities;
using EcommerceWebApi.Utilities;

namespace EcommerceWebApi.Services
{
    public interface IProductService
    {
        Task<bool> DeleteProductAsync(int id);
        void Dispose();
        List<Product> GetAllProducts();
        int GetNextProductId();
        List<Product> GetPaginationProducts(
            PaginationFilter paginationFilter,
            QueryFilter queryFilter,
            out int queryProductCount
        );
        Product? GetProductById(int id);
        Task<bool> InsertProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> UpdateProductPropertyAsync<T>(Product product, string property, T value);
    }
}
