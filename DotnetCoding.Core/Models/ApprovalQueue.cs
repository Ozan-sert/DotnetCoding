namespace DotnetCoding.Core.Models;

public class ApprovalQueue
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string RequestReason { get; set; }
    public DateTime RequestDate { get; set; }
     public Product Product { get; set; }
    // Navigation property for Product
   
}
