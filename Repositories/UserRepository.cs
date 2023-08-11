using EcommerceWebApi.Entities;
using JsonFlatFileDataStore;

namespace EcommerceWebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly DataStore _store;

        public UserRepository(UnitOfWork unitOfWork, DataStore store)
        {
            _unitOfWork = unitOfWork;
            _store = store;
        }

        public IDocumentCollection<User> GetAll()
        {
            try
            {
                return _store.GetCollection<User>();
            }
            catch
            {
                throw;
            }
        }

        public User? GetById(string id)
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

        public User? GetByToken(string token)
        {
            try
            {
                return GetAll().AsQueryable().FirstOrDefault(x => x.RefreshToken.Token == token);
            }
            catch
            {
                throw;
            }
        }

        public User? GetByName(string name)
        {
            try
            {
                return GetAll().AsQueryable().FirstOrDefault(x => x.Username == name);
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> InsertAsync(User user)
        {
            try
            {
                if (_unitOfWork.IsTransactionInProgress())
                {
                    _unitOfWork.AddToTransaction(() => GetAll().InsertOne(user));
                    return Task.FromResult(true);
                }
                else
                {
                    return GetAll().InsertOneAsync(user);
                }
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> UpdateAsync(User user)
        {
            try
            {
                if (_unitOfWork.IsTransactionInProgress())
                {
                    _unitOfWork.AddToTransaction(() => GetAll().UpdateOne(user.Id, user));
                    return Task.FromResult(true);
                }
                else
                {
                    return GetAll().UpdateOneAsync(user.Id, user);
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
