using DotnetCoding.Core.Models;

namespace DotnetCoding.Core.Interfaces;

public interface IApprovalQueueRepository : IGenericRepository<ApprovalQueue>
{
    // You can add additional methods specific to the ApprovalQueue entity if needed
    Task<IEnumerable<ApprovalQueue>> SearchApprovalQueueItemsAsync(string productName, DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<ApprovalQueue>> FindAllWithIncludeAsync();
}