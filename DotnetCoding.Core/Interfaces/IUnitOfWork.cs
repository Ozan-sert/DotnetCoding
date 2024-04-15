
namespace DotnetCoding.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IApprovalQueueRepository ApprovalQueues { get; }
        IProductHistoryRepository ProductHistories { get; }
        Task<int> SaveAsync();
    }
}