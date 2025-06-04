using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.ProductDTO;
using Noja.Core.Interfaces.Services;
using Noja.Infrastructure.Data;
using Noja.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace Noja.Application.Services.Products
{
    public class ProductService : IProductService
    {

        private readonly NojaDbContext _context;

        public ProductService(NojaDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<AdminProductDto>> CreateProductAsync(CreateProductDto createproductDto, string adminId)
        {
            var response = new ServiceResponse<AdminProductDto>();

            try
            {
                var product = new Product
                {
                    Name = createproductDto.Name,
                    Description = createproductDto.Description,
                    Price = createproductDto.Price,
                    Category = createproductDto.Category,
                    UnitOfMeasure = createproductDto.UnitOfMeasure,
                    UnitSize = createproductDto.UnitSize,
                    Quantity = createproductDto.Quantity,
                    CreatedBy = adminId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Product created successfully";
                response.Data = MapToAdminProductDto(product);
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error creating produc:{ex.Message}";
                return response;
            }
        }   

        public async Task<ServiceResponse<string>> DeleteProductAsync(Guid id, string adminId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;
                }

                product.IsActive = false; // soft delete
                product.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Product deleted successfully";
                response.Data = "Product deactivate";
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting product: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<ProductSummaryDto>>> GetActiveProductsForCustomerAsync()
        {
            var response = new ServiceResponse<List<ProductSummaryDto>>();

            try
            {
                var products = await _context.Products
                .Where(p => p.IsActive && p.Quantity > 0)
                .OrderBy(p => p.Name)
                .ToListAsync();

                response.Success = true;
                response.Data = products.Select(MapToProductSummaryDto).ToList();
                response.Message = $"Retrieved {products.Count} active products";
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving products {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<AdminProductDto>>> GetAllProductsForAdminAsync(string adminId)
        {
            var response = new ServiceResponse<List<AdminProductDto>>();

            try
            {
                var adminExists = await _context.Users.AnyAsync(u => u.Id == adminId);
                if(!adminExists)
                {
                    response.Success = false;
                    response.Message = "Invalid admin ID";
                    return response;
                }

                var products = await _context.Products
                .Include(p => p.CreatedByUser)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

                response.Success = true;
                response.Message = $"Retrieved {products.Count} products";
                response.Data = products.Select(MapToAdminProductDto).ToList();
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving products: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<ProductDto>> GetProductDetailsForCustomerAsync(Guid id)
        {
            var response = new ServiceResponse<ProductDto>();

            try
            {
                var products = await _context.Products
                .Where(p => p.Id == id && p.IsActive)
                .FirstOrDefaultAsync();

                if (products == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;   
                }

                if (products.Quantity <= 0)
                {
                    // still shows product but indicate it's out of stock
                    response.Success = true;
                    response.Message = "Product is currently out of stock";
                    response.Data = MapToProductDto(products);
                    return response;
                }

                response.Success = true;
                response.Message = "Product retrieved successfully";
                response.Data = MapToProductDto(products);
                return response;


            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving product: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<AdminProductDto>> GetProductForAdminAsync(Guid id, string adminId)
        {
            var response = new ServiceResponse<AdminProductDto>();

            try
            {
                var adminExists = await _context.Users.AnyAsync(u => u.Id == adminId);
                if(!adminExists)
                {
                    response.Success = false;
                    response.Message = "Invalid admin ID";
                    return response;
                }

                var product = await _context.Products
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;   
                }
                response.Success = true;
                response.Message = "Product retrieved successfully";
                response.Data = MapToAdminProductDto(product);
                return response;
                
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error searching products: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<AdminProductDto>>> SearchProductsForAdmin(string searchTerm, string adminId)
        {
            var response = new ServiceResponse<List<AdminProductDto>>();

            try
            {
                var adminExists = await _context.Users.AnyAsync(u => u.Id == adminId);
                if(!adminExists)
                {
                    response.Success = false;
                    response.Message = "Invalid admin ID";
                    return response;
                }

                searchTerm = searchTerm.ToLower().Trim();

                // split search term
                var searchWords = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var products = await _context.Products
                .Include(p => p.CreatedByUser)
                .Where(p => searchWords.Any(word =>
                    p.Name.ToLower().Contains(word) ||
                    p.Description.ToLower().Contains(word) ||
                    p.Category.ToString().ToLower().Contains(word)
                ))
                .OrderBy(p => p.Name.ToLower().StartsWith(searchTerm) ? 0 : 1)
                // prioritize product Names
                .ThenBy(p => p.Name)
                .ToListAsync();

                response.Success = true;
                response.Data = products.Select(MapToAdminProductDto).ToList();
                response.Message = products.Count > 0
                ? $"Found {products.Count} products matching search term '{searchTerm}'"
                : $"No products found matching search term '{searchTerm}'";
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error searching products: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<ProductSummaryDto>>> SearchProductsForCustomersAsync(string searchTerm)
        {
            var response = new ServiceResponse<List<ProductSummaryDto>>();

            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    response.Success = false;
                    response.Message = "Search term can't be empty";
                    return response;
                }

                searchTerm = searchTerm.ToLower().Trim();

                // split search term
                var searchWords = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var products = await _context.Products
                .Where(p => p.IsActive && p.Quantity > 0)
                .Where(p => searchWords.Any(word =>
                    p.Name.ToLower().Contains(word) ||
                    p.Description.ToLower().Contains(word) ||
                    p.Category.ToString().ToLower().Contains(word)
                ))
                .OrderBy(p => p.Name.ToLower().StartsWith(searchTerm) ? 0 : 1)
                // prioritize product Names
                .ThenBy(p => p.Name)
                .ToListAsync();

                response.Success = true;
                response.Data = products.Select(MapToProductSummaryDto).ToList();
                response.Message = products.Count > 0
                ? $"Found {products.Count} products matching search term '{searchTerm}'"
                : $"No products found matching search term '{searchTerm}'";
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error searching products: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<AdminProductDto>> UpdateProductAsync(Guid id, UpdateProductDto updateproductDto, string adminId)
        {
            var response = new ServiceResponse<AdminProductDto>();
            try
            {
                var product = await _context.Products
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;   
                }

                var admin  = product.CreatedBy; // Logs the admin who made the change

                product.Name = updateproductDto.Name;
                product.Description = updateproductDto.Description;
                product.Category = updateproductDto.Category;
                product.UnitOfMeasure = updateproductDto.UnitOfMeasure;
                product.UnitSize = updateproductDto.UnitSize;
                product.Price = updateproductDto.Price;
                product.Quantity = updateproductDto.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Product updated successfully";
                response.Data = MapToAdminProductDto(product);
                return response;

            }
            catch (DbUpdateConcurrencyException)
            {
                response.Success = false;
                response.Message = "Product was modified by another Admin";
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating product: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<AdminProductDto>> UpdateProductStockAsync(Guid id, int newQuantity, string adminId)
        {
            var response = new ServiceResponse<AdminProductDto>();

            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;   
                }

                var oldQuantity = product.Quantity;
                product.Quantity = newQuantity;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = $"Stock updated from {oldQuantity} to {newQuantity}";
                response.Data = MapToAdminProductDto(product);
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating stock:{ex.Message}";
                return response;
            }
        }


        // Mapping methods

        private static AdminProductDto MapToAdminProductDto(Product product)
        {
            return new AdminProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                UnitOfMeasure = product.UnitOfMeasure,
                Quantity = product.Quantity,
                IsActive = product.IsActive,
                UpdatedAt = product.UpdatedAt,
                CreatedBy = product.CreatedBy,
                CreatedAt = product.CreatedAt,

                // Properties to display
                DisplayName = product.DisplayName,
                UnitPriceDisplay = product.UnitPriceDisplay,
                PackagePriceDisplay = product.PackagePriceDisplay,
                FullPriceDisplay = product.FullPriceDisplay,
                CategoryDisplay = product.CategoryDisplay,
                IsInStock = product.IsInStock,
                StockStatusDisplay = product.StockStatusDisplay,
                UnitAbbreviation = product.UnitAbbreviation
            };
        }

        private static ProductSummaryDto MapToProductSummaryDto(Product product)
        {
            return new ProductSummaryDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category,
                IsInStock = product.IsInStock,
                DisplayName = product.DisplayName,
                UnitPriceDisplay = product.UnitPriceDisplay,
                StockStatusDisplay = product.StockStatusDisplay,
            };
        }

        private static ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                UnitOfMeasure = product.UnitOfMeasure,
                UnitSize = product.UnitSize,
                Quantity = product.Quantity,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,

                // customer display
                DisplayName = product.DisplayName,
                UnitPriceDisplay = product.UnitPriceDisplay,
                PackagePriceDisplay = product.PackagePriceDisplay,
                FullPriceDisplay = product.FullPriceDisplay,
                StockStatusDisplay = product.StockStatusDisplay,
                 CategoryDisplay = product.CategoryDisplay,
                IsInStock = product.IsInStock,
                // PackagePrice = product.PackagePrice,
                UnitAbbreviation = product.UnitAbbreviation

            };
        }
    }
   
}