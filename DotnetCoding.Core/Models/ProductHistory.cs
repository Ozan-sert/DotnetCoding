namespace DotnetCoding.Core.Models;

public class ProductHistory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Price { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public Product Product { get; set; }

}