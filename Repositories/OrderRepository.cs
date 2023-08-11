using EcommerceWebApi.Entities;
using JsonFlatFileDataStore;

namespace EcommerceWebApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly DataStore _store;

        public OrderRepository(UnitOfWork unitOfWork, DataStore store)
        {
            _unitOfWork = unitOfWork;
            _store = store;
        }

        public IDocumentCollection<Order> GetAll()
        {
            try
            {
                return _store.GetCollection<Order>();
            }
            catch
            {
                throw;
            }
        }

        public Order? GetById(string id)
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

        public Task<bool> InsertAsync(Order order)
        {
            try
            {
                if (_unitOfWork.IsTransactionInProgress())
                {
                    _unitOfWork.AddToTransaction(() => GetAll().InsertOne(order));
                    return Task.FromResult(true);
                }
                else
                {
                    return GetAll().InsertOneAsync(order);
                }
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> UpdateAsync(Order order)
        {
            try
            {
                if (_unitOfWork.IsTransactionInProgress())
                {
                    _unitOfWork.AddToTransaction(() => GetAll().UpdateOne(order.Id, order));
                    return Task.FromResult(true);
                }
                else
                {
                    return GetAll().UpdateOneAsync(order.Id, order);
                }
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> DeleteAsync(string id)
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
