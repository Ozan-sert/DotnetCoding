using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.DTOs;

namespace DotnetCoding.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string productName, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate);
        Task<Product> CreateProductAsync(ProductDTO productDto);
        Task<Product> UpdateProductAsync(int productId, ProductDTO updatedProductDto);
        Task<bool> DeleteProductAsync(int productId);
    }
}
