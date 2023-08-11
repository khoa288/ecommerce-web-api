using EcommerceWebApi.Entities;
using EcommerceWebApi.Notification;
using EcommerceWebApi.Utilities;
using System.Reflection;

namespace EcommerceWebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Product> GetAllProducts()
        {
            try
            {
                return _unitOfWork.Products.GetAll().AsQueryable().ToList();
            }
            catch
            {
                throw;
            }
        }

        public int GetNextProductId()
        {
            return _unitOfWork.Products.GetAll().GetNextIdValue();
        }

        public List<Product> GetPaginationProducts(
            PaginationFilter paginationFilter,
            QueryFilter queryFilter,
            out int queryProductCount
        )
        {
            // Get queryable product collection
            var products = GetAllProducts();

            // Search products
            products = QueryHelper
                .SearchObjects(
                    products,
                    queryFilter.SearchBy,
                    queryFilter.Search,
                    StringComparison.OrdinalIgnoreCase
                )
                .ToList();

            // Sort products
            products = QueryHelper
                .SortObjects(products, queryFilter.SortBy, queryFilter.IsSortAscending)
                .ToList();

            // Get current quantity
            queryProductCount = products.Count;

            return products
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize)
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            return _unitOfWork.Products.GetById(id);
        }

        public async Task<bool> InsertProductAsync(Product product)
        {
            try
            {
                var result = await _unitOfWork.Products.InsertAsync(product);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var result = await _unitOfWork.Products.UpdateAsync(product);
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateProductPropertyAsync<T>(
            Product product,
            string property,
            T value
        )
        {
            var propertyInfo = typeof(Product).GetProperty(
                property,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (propertyInfo == null)
            {
                return false;
            }
            try
            {
                propertyInfo.SetValue(product, value, null);
                return await UpdateProductAsync(product);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.Products.DeleteAsync(id);
                _unitOfWork.CommitTransaction();
                return result;
            }
            catch
            {
                throw;
            }
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
