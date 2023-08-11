using EcommerceWebApi.Entities;
using JsonFlatFileDataStore;

namespace EcommerceWebApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly DataStore _store;

        public ProductRepository(UnitOfWork unitOfWork, DataStore store)
        {
            _unitOfWork = unitOfWork;
            _store = store;
        }

        public IDocumentCollection<Product> GetAll()
        {
            try
            {
                return _store.GetCollection<Product>();
            }
            catch
            {
                throw;
            }
        }

        public Product? GetById(int id)
        {
            try
            {
                return GetAll().AsQueryable().FirstOrDefault(x => x.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> InsertAsync(Product product)
        {
            try
            {
                if (_unitOfWork.IsTransactionInProgress())
                {
                    _unitOfWork.AddToTransaction(() => GetAll().InsertOne(product));
                    return Task.FromResult(true);
                }
                else
                {
                    return GetAll().InsertOneAsync(product);
                }
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> UpdateAsync(Product product)
        {
            try
            {
                if (_unitOfWork.IsTransactionInProgress())
                {
                    _unitOfWork.AddToTransaction(() => GetAll().UpdateOne(product.Id, product));
                    return Task.FromResult(true);
                }
                else
                {
                    return GetAll().UpdateOneAsync(product.Id, product);
                }
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (_unitOfWork.IsTransactionInProgress())
                {
                    _unitOfWork.AddToTransaction(() => GetAll().DeleteOne(id));
                    return Task.FromResult(true);
                }
                else
                {
                    return GetAll().DeleteOneAsync(id);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
