using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Services;

public class ApprovalQueueService : IApprovalQueueService
{
    private readonly IUnitOfWork _unitOfWork;

    public ApprovalQueueService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ApprovalQueue>> GetApprovalQueueItemsAsync()
    {
        var approvalQueueItems = await _unitOfWork.ApprovalQueues.FindAllWithIncludeAsync();
        return approvalQueueItems;
    }

    public async Task<bool> ApproveProductAsync(int approvalQueueItemId)
    {
        var approvalQueueItem = await _unitOfWork.ApprovalQueues.GetByIdAsync(approvalQueueItemId);

        if (approvalQueueItem == null)
        {
            throw new InvalidOperationException($"ApprovalQueue item with ID {approvalQueueItemId} not found");
        }
        // Perform approval logic
        var product = await _unitOfWork.Products.GetByIdAsync(approvalQueueItem.ProductId);
        // Check if the reason is delete
        if (approvalQueueItem.RequestReason.Equals("deleted", StringComparison.OrdinalIgnoreCase))
        {
            // Simply remove the item from the approval queue
            await _unitOfWork.ApprovalQueues.DeleteAsync(approvalQueueItem.Id);
           
            //delete product
            await _unitOfWork.Products.DeleteAsync(product.Id);
            // Save changes to the database
            await _unitOfWork.SaveAsync();

            return true;
        }
        
        // Set product as active
        product.IsActive = true;
        await _unitOfWork.Products.UpdateAsync(product);

        // Remove item from approval queue
        await _unitOfWork.ApprovalQueues.DeleteAsync(approvalQueueItem.Id);

        // Save changes to the database
        await _unitOfWork.SaveAsync();

        return true;
    }

    public async Task<bool> RejectProductAsync(int approvalQueueItemId)
    {
        var approvalQueueItem = await _unitOfWork.ApprovalQueues.GetByIdAsync(approvalQueueItemId);

        if (approvalQueueItem == null)
        {
            throw new InvalidOperationException($"ApprovalQueue item with ID {approvalQueueItemId} not found");
        }

        // Check if the reason is delete
        if (approvalQueueItem.RequestReason.Equals("deleted", StringComparison.OrdinalIgnoreCase))
        {
            // Simply remove the item from the approval queue
            await _unitOfWork.ApprovalQueues.DeleteAsync(approvalQueueItem.Id);
            // Perform approval logic
            var product = await _unitOfWork.Products.GetByIdAsync(approvalQueueItem.ProductId);
            // Set product as active
            product.IsActive = true;
            // Save changes to the database
            await _unitOfWork.SaveAsync();

            return true;
        }

        // Retrieve the original state from ProductHistory
        
        var previousProduct = (await _unitOfWork.ProductHistories
                .FindAsync(ph => ph.ProductId == approvalQueueItem.ProductId))
                .FirstOrDefault();

        if (previousProduct == null)
        {
            throw new InvalidOperationException($"ProductHistory for Product ID {approvalQueueItem.ProductId} not found");
        }

       

        // Apply the original state to Products
        var originalProduct = await _unitOfWork.Products.GetByIdAsync(approvalQueueItem.ProductId);
        originalProduct.Name = previousProduct.Name;
        originalProduct.Description = previousProduct.Description;
        originalProduct.Price = previousProduct.Price;
        originalProduct.IsActive = true; // If it's a delete, set IsActive based on history

        // Remove item from approval queue
        await _unitOfWork.ApprovalQueues.DeleteAsync(approvalQueueItem.Id);
        
        // Remove item from product history
        await _unitOfWork.ProductHistories.DeleteAsync(previousProduct.Id);
        // Save changes to the database
        await _unitOfWork.SaveAsync();

        return true;
    }

}
