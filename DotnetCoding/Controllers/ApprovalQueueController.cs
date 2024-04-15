    using DotnetCoding.Core.Interfaces;
    using DotnetCoding.Core.Models;
    using DotnetCoding.DTOs;
    using DotnetCoding.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCoding.Controllers
{
    [ApiController]
    [Route("api/approval")]
    public class ApprovalQueueController : ControllerBase
    {
        private readonly IApprovalQueueService _approvalQueueService;
        private readonly IProductHistoryRepository _productHistoryRepository;

        public ApprovalQueueController(IApprovalQueueService approvalQueueService, IProductHistoryRepository productHistoryRepository)
        {
            _approvalQueueService = approvalQueueService;
            _productHistoryRepository = productHistoryRepository;
        }

        [HttpGet("items")]
        public async Task<ActionResult<IEnumerable<ApprovalQueueResponseDTO>>> GetApprovalQueueItems()
        {
        
            var approvalQueueItems = await _approvalQueueService.GetApprovalQueueItemsAsync();
            
            var orderedApprovalQueueItems = approvalQueueItems
                .OrderBy(aq => aq.RequestDate)
                .Select(aq => new ApprovalQueueResponseDTO()
                {
                    Id = aq.Id,
                    ProductId = aq.ProductId,
                    RequestReason = aq.RequestReason,
                    RequestDate = aq.RequestDate,
                    // Populate additional properties from the related Product
                    ProductName = aq.Product?.Name!,
                 
                });

            return Ok(orderedApprovalQueueItems);
        }

        [HttpPost("approve/{approvalQueueItemId}")]
        public async Task<ActionResult<bool>> ApproveProduct(int approvalQueueItemId)
        {
            try
            {
                var result = await _approvalQueueService.ApproveProductAsync(approvalQueueItemId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reject/{approvalQueueItemId}")]
        public async Task<ActionResult<bool>> RejectProduct(int approvalQueueItemId)
        {
            try
            {
                var result = await _approvalQueueService.RejectProductAsync(approvalQueueItemId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}