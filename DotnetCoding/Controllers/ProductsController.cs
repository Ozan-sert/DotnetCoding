using Microsoft.AspNetCore.Mvc;
using DotnetCoding.Core.Models;
using DotnetCoding.DTOs;
using DotnetCoding.Services.DTOs;
using DotnetCoding.Services.Interfaces;

namespace DotnetCoding.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetActiveProducts()
        {
            var activeProducts = await _productService.GetActiveProductsAsync();

            // Order the active products by PostedDate in descending order
            var orderedActiveProducts = activeProducts.OrderByDescending(p => p.PostedDate)
                .Select(p => new ProductResponseDTO()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    PostedDate = p.PostedDate
                });

            return Ok(orderedActiveProducts);
        }
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>>  GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            
            return Ok(products.OrderByDescending(p=>p.PostedDate));
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductResponseDTO>>>  SearchProducts([FromQuery] string? productName, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var products = await _productService.SearchProductsAsync(productName, minPrice, maxPrice, startDate, endDate);
            // Filter and map to ProductResponseDTO
            var activeProducts = products
                .Where(p => p.IsActive)
                .OrderByDescending(p=>p.PostedDate)
                .Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    PostedDate = p.PostedDate
                });
            return Ok(activeProducts);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDTO>> CreateProduct([FromBody] ProductDTO productDto)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(productDto);
                return CreatedAtAction(nameof(GetAllProducts), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(int productId, [FromBody] ProductDTO updatedProductDto)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(productId, updatedProductDto);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
