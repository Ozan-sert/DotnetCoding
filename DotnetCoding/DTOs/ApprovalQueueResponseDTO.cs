namespace DotnetCoding.DTOs;

public class ApprovalQueueResponseDTO
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string RequestReason { get; set; }
    public DateTime RequestDate { get; set; }

    // Properties from the related Product
    public string ProductName { get; set; }
}