using DotnetCoding.Core.Models;

namespace DotnetCoding.Services.Interfaces;

public interface IApprovalQueueService
{
    Task<IEnumerable<ApprovalQueue>> GetApprovalQueueItemsAsync();
    Task<bool> ApproveProductAsync(int approvalQueueItemId);
    Task<bool> RejectProductAsync(int approvalQueueItemId);
}