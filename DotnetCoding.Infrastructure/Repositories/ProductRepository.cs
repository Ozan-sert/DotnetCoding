using System.Linq.Expressions;
using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;

namespace DotnetCoding.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DbContextClass dbContext) : base(dbContext)
        {

        }
        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await FindAsync(p => p.IsActive == true);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(
            string? productName, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            return await FindAsync(p =>
                (string.IsNullOrEmpty(productName) || p.Name.Contains(productName)) &&
                (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                (!startDate.HasValue || p.PostedDate >= startDate.Value) &&
                (!endDate.HasValue || p.PostedDate <= endDate.Value)
            );
        }
    }
}
