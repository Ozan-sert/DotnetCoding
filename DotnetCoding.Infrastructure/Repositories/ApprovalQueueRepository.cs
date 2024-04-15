using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoding.Infrastructure.Repositories;

public class ApprovalQueueRepository : GenericRepository<ApprovalQueue>, IApprovalQueueRepository
{
    public ApprovalQueueRepository(DbContextClass context) : base(context)
    {
    }
    
    public async Task<IEnumerable<ApprovalQueue>> FindAllWithIncludeAsync()
    {
        return await DbContext.Set<ApprovalQueue>()
            .Include(aq => aq.Product)
            .ToListAsync();
    }
    public async Task<IEnumerable<ApprovalQueue>> SearchApprovalQueueItemsAsync(string productName, DateTime? startDate,
        DateTime? endDate)
    {
       
        return await FindAsync(a =>
            (string.IsNullOrEmpty(productName) || a.Product.Name.Contains(productName)) &&
            (!startDate.HasValue || a.RequestDate >= startDate.Value) &&
            (!endDate.HasValue || a.RequestDate <= endDate.Value));
    }
}
