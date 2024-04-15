using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Infrastructure.Repositories;

public class ProductHistoryRepository :GenericRepository<ProductHistory>,IProductHistoryRepository    
{
    public ProductHistoryRepository(DbContextClass context) : base(context)
    {
    }
}