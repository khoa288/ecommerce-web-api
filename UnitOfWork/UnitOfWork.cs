using EcommerceWebApi.Repositories;
using JsonFlatFileDataStore;

namespace EcommerceWebApi
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataStore _store;
        private ChangeSet? _currentTransaction;
        private bool _disposed = false;

        public IUserRepository Users { get; private set; }

        public IProductRepository Products { get; private set; }

        public IOrderRepository Orders { get; private set; }

        public UnitOfWork()
        {
            _store = new DataStore("database.json");
            Users = new UserRepository(this, _store);
            Products = new ProductRepository(this, _store);
            Orders = new OrderRepository(this, _store);
        }

        public bool IsTransactionInProgress()
        {
            return _currentTransaction != null;
        }

        public void StartTransaction()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _currentTransaction = new ChangeSet();
        }

        public void CommitTransaction()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            foreach (var action in _currentTransaction.Actions)
            {
                action();
            }

            _currentTransaction = null;
        }

        public void AbortTransaction()
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            _currentTransaction = null;
        }

        internal void AddToTransaction(Action action)
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            _currentTransaction.Actions.Add(action);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _store?.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }

    public class ChangeSet
    {
        public List<Action> Actions { get; } = new List<Action>();
    }
}
