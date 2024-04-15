using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotnetCoding.Core.Interfaces;
using DotnetCoding.Core.Models;
using DotnetCoding.Services.DTOs;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var productDetailsList = await _unitOfWork.Products.GetAllAsync();
            return productDetailsList;
        }


        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            // Implement logic to get active products (e.g., products with status "Active")
            var activeProducts = await _unitOfWork.Products.GetActiveProductsAsync();
            return activeProducts;
        }
        public async Task<IEnumerable<Product>> SearchProductsAsync(string? productName, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            // Implement logic to search products based on the provided criteria
            var searchResults = await _unitOfWork.Products.SearchProductsAsync(productName, minPrice, maxPrice, startDate, endDate);
            return searchResults;
        }

        public async Task<Product> CreateProductAsync(ProductDTO productDto)
        {
            // Business logic for creating product
            if (productDto.Price > 10000)
            {
                // Decline the product
                // Implement appropriate handling, e.g., throw an exception or return a result indicating the decline
                throw new InvalidOperationException("Product creation declined. Price exceeds $10,000");
            }
            // Map DTO properties to Product entity
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                PostedDate = DateTime.UtcNow, // Set posted date as needed
            };
            
            if (product.Price > 5000)
            {
                // Push to approval queue
                product.IsActive = false; // Set status as needed
                

                await _unitOfWork.Products.AddAsync(product);
               

                var approvalQueueItem = new ApprovalQueue
                {
                    ProductId = product.Id,
                    RequestReason = "created",
                    RequestDate = DateTime.UtcNow
                };
                await _unitOfWork.ApprovalQueues.AddAsync(approvalQueueItem);
                await _unitOfWork.SaveAsync();

                return product;
            }
            else
            {
                // Store product in the main product table
                product.IsActive = true; // Set status as needed
                

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveAsync();

                return product;
            }
        }

        public async Task<Product> UpdateProductAsync(int productId, ProductDTO updatedProductDto)
        {
            // Retrieve the existing product by ID
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(productId);

            if (existingProduct is null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found");
            }

            if (existingProduct.IsActive == false)
            {
                throw new InvalidOperationException($"This product is already in approvalqueue");
            }

            // Create a copy of the existing product to capture its state before the update
            var previousProduct = new Product
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                Price = existingProduct.Price,
                PostedDate = existingProduct.PostedDate,
                IsActive = existingProduct.IsActive
                // Add any other properties as needed
            };

         
            
            // Map updatedProductDto properties to existingProduct
            existingProduct.Name = updatedProductDto.Name;
            existingProduct.Description = updatedProductDto.Description;
            existingProduct.Price = updatedProductDto.Price;
            existingProduct.PostedDate = DateTime.UtcNow; // Set posted date as needed
            

            // Business logic for updating product
            if (existingProduct.Price > (decimal)(1.5 * previousProduct.Price) || 
                existingProduct.Price > 5000)
            {
                // Add a record to ProductHistory for the rejected update
                await AddProductHistoryAsync(previousProduct);

                // Push to approval queue if the price increase is more than 50%
                existingProduct.IsActive = false; // Set status as needed
              

                var approvalQueueItem = new ApprovalQueue
                {
                    ProductId = existingProduct.Id,
                    RequestReason = "updated",
                    RequestDate = DateTime.UtcNow
                };
                await _unitOfWork.ApprovalQueues.AddAsync(approvalQueueItem);

          
            }
         
                // Update the existing product in the database
                await _unitOfWork.SaveAsync();
            

            return existingProduct;
        }
        private async Task AddProductHistoryAsync(Product product)
        {
            var productHistory = new ProductHistory
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
               
            };

            await _unitOfWork.ProductHistories.AddAsync(productHistory);
            await _unitOfWork.SaveAsync();
        }
        public async Task<bool> DeleteProductAsync(int productId)
        {
            // Retrieve the existing product by ID
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(productId);

            if (existingProduct is null)
            {
                // Handle the case where the product with the given ID is not found
                // You can throw an exception or return an appropriate result
                throw new InvalidOperationException($"Product with ID {productId} not found");
            }
            if (existingProduct.IsActive == false)
            {
                throw new InvalidOperationException($"This product is already in approvalqueue");
            }
            // Business logic for deleting product
            // Push to approval queue
            existingProduct.IsActive = false; // Set status as needed
           

            var approvalQueueItem = new ApprovalQueue
            {
                ProductId = existingProduct.Id,
                RequestReason = "deleted",
                RequestDate = DateTime.UtcNow
            };
            await _unitOfWork.ApprovalQueues.AddAsync(approvalQueueItem);
            
            await _unitOfWork.SaveAsync();

            return true; // Return success or appropriate result
        }

    }
}
