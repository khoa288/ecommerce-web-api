using EcommerceWebApi.Repositories;

namespace EcommerceWebApi
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }

        void StartTransaction();

        void CommitTransaction();

        void AbortTransaction();

        bool IsTransactionInProgress();
    }
}
