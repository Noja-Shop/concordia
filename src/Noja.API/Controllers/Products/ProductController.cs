using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Noja.Application.Models.ProductDTO;
using Noja.Application.Services.Products;
using Noja.Core.Interfaces.Services;
using Noja.Core.Models.ProductDTO;

namespace Noja.API.Controllers.Products
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Admin controllers

        [Authorize]
        [HttpPost(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.CreateProduct)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto create)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productService.CreateProductAsync(create, adminId);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetProductByIdForAdmin), new {id = result.Data.Id}, result);
            
        }
        
        [Authorize]
        [HttpGet(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.GetProductById)]
        public async Task<IActionResult> GetProductByIdForAdmin(Guid id)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _productService.GetProductForAdminAsync(id, adminId);
            if (!result.Success)
             return NotFound(result);
            
            return Ok(result);
        }

        [Authorize]
        [HttpPut(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.UpdateProduct)]  
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto update)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productService.UpdateProductAsync(id, update, adminId);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }  

        [Authorize]
        [HttpGet(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.GetAllProducts)]
        public async Task<IActionResult> GetAllProductsForAdmin()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productService.GetAllProductsForAdminAsync(adminId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPatch(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.UpdateProductStock)]
        public async Task<IActionResult> UpdateProductStock(Guid id, [FromBody] UpdateStockDto updateStock)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _productService.UpdateProductStockAsync(id, updateStock.Quantity, adminId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }

        [Authorize]
        [HttpDelete(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.DeleteProduct)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productService.DeleteProductAsync(id, adminId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.GetProductForAdminBySearch)]
        public async Task<IActionResult> SearchProductsForAdmin([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new {
                    success = false,
                    message = "Search term 'q' is required"
                });
            }

            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _productService.SearchProductsForAdmin(q, adminId);

            if (!result.Success) return BadRequest(result);
            return Ok(result);

        }

        // ========== Customer Endpoints ============= //

        [HttpGet(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.GetActiveProductForCustomer)]
        public async Task<IActionResult> SearchAllActiveProductsForCustomer()
        {
            var result = await _productService.GetActiveProductsForCustomerAsync();

            if (!result.Success) return BadRequest(result);

            return Ok(result);
            
        }

        [HttpGet(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.SearchProductsForCustomer)]
        public async Task<IActionResult> SearchProductsForCustomer([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new {
                    success = false,
                    message = "Search term 'q' is required"
                });
            }
            var result = await _productService.SearchProductsForCustomersAsync(q);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet(Endpoints.ProductEndpoints.ProductsAPIEndpoints.Product.GetProductDetailsForCustomer)]
        public async Task<IActionResult> SearchProductDetailsForCustomer(Guid id)
        {
            var result = await _productService.GetProductDetailsForCustomerAsync(id);

            if(!result.Success) return NotFound(result);
            return Ok(result);

        }
          
    }
}