using JsonFlatFileDataStore;
using LoginForm.Entities;
using LoginForm.Utils;

namespace LoginForm.Services
{
    public class ProductService
    {
        private readonly DataStore _store;
        private readonly string _filePath = "Database/Products.json";
        private int queryProductCount = 0;

        public ProductService()
        {
            _store = new DataStore(_filePath);
        }

        private IDocumentCollection<Product> GetProductCollection()
        {
            var collection = _store.GetCollection<Product>();
            _store.Dispose();
            return collection;
        }

        public async Task<List<Product>> GetProductsAsync(
            PaginationFilter paginationFilter,
            QueryFilter queryFilter
        )
        {
            // Get queryable product collection
            var products = GetProductCollection().AsQueryable().ToList();

            // Filter by search query
            if (!string.IsNullOrEmpty(queryFilter.Search))
            {
                products = products
                    .Where(
                        p =>
                            p.Title.Contains(queryFilter.Search, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            // Sort products
            if (!string.IsNullOrEmpty(queryFilter.SortBy))
            {
                switch (queryFilter.SortBy.ToLower())
                {
                    case "rating":
                        products = queryFilter.IsSortAscending
                            ? products.OrderBy(p => p.Rating).ToList()
                            : products.OrderByDescending(p => p.Rating).ToList();
                        break;
                    case "price":
                        products = queryFilter.IsSortAscending
                            ? products.OrderBy(p => p.Price).ToList()
                            : products.OrderByDescending(p => p.Price).ToList();
                        break;
                    default:
                        break;
                }
            }

            // Get current quantity
            queryProductCount = products.Count;

            return await Task.FromResult(
                products
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize)
                    .ToList()
            );
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await Task.FromResult(
                GetProductCollection().AsQueryable().FirstOrDefault(p => p.Id == id)
            );
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            var collection = GetProductCollection();
            product.Id = collection.GetNextIdValue();
            return await collection.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            return await GetProductCollection().UpdateOneAsync(product.Id, product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await GetProductCollection().DeleteOneAsync(p => p.Id == id);
        }

        public int TotalProductCount()
        {
            return GetProductCollection().Count;
        }

        public int QueryProductCount()
        {
            return queryProductCount;
        }
    }
}
