using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.ProductDTO;
using Noja.Core.Entity;
using Noja.Core.Interfaces.Repository;
using Noja.Core.Interfaces.Service;

namespace Noja.Application.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
                    PackagePrice = createproductDto.PackagePrice,
                    Category = createproductDto.Category,
                    UnitPrice = createproductDto.UnitPrice,
                    ContainerSize = createproductDto.ContainerSize,
                    PackageSize = createproductDto.PackageSize,
                    PackageType = createproductDto.PackageType,
                    ContainerType = createproductDto.ContainerType,
                    MeasurementUnit = createproductDto.MeasurementUnit,
                    ContainerCount = createproductDto.ContainerCount,
                    Quantity = createproductDto.Quantity,
                    CreatedBy = adminId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createdProduct = await _productRepository.CreateProductAsync(product);
                response.Success = true;
                response.Message = "Product created successfully";
                response.Data = MapToAdminProductDto(createdProduct);
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error creating product: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<AdminProductDto>> DeleteProductAsync(Guid id, string adminId)
        {
            var response = new ServiceResponse<AdminProductDto>();
            try
            {
                var product = await _productRepository.GetProductById(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;   
                }

                var productDto = MapToAdminProductDto(product);

                var deleteSuccess = await _productRepository.DeleteProductAsync(id);

                if (!deleteSuccess)
                {
                    response.Success = false;
                    response.Message = "Failed to delete product";
                    return response;
                }

                response.Success = true;
                response.Message = "Product deleted successfully";
                response.Data = productDto;
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting product: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<ProductDto>>> GetActiveProductsAsync()
        {
            var response = new ServiceResponse<List<ProductDto>>();

            try
            {
                var products = await _productRepository.GetActiveProductsAsync();

                response.Success = true;
                response.Data = products.Select(MapToProductDto).ToList();
                response.Message = $"Successfully retrieved {products.Count} products";
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving products {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<ProductDto>>> GetAllProductsAsync()
        {
            var response = new ServiceResponse<List<ProductDto>>();
            try
            {
                var products = await _productRepository.GetActiveProductsAsync();

                response.Success = true;
                response.Data = products.Select(MapToProductDto).ToList();
                response.Message = $"Successfully retrieved {products.Count} products";
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving products {ex.Message}";
                return response;
            }

        }

        public async Task<ServiceResponse<ProductDto>> GetProductDetailsForCustomerAsync(Guid id)
        {
            var response = new ServiceResponse<ProductDto>();

            try
            {
                var product = await _productRepository.GetProductById(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;   
                }

                if (product.Quantity <= 0)
                {
                    // still shows product but indicate it's out of stock
                    response.Success = true;
                    response.Message = "Product is currently out of stock";
                    response.Data = MapToProductDto(product);
                    return response;
                }

                response.Success = true;
                response.Message = "Product details retrieved successfully";
                response.Data = MapToProductDto(product);
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
                var product = await _productRepository.GetProductById(id);
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

        public Task<ServiceResponse<List<ProductDto>>> GetProductsByCategoryAsync(string category)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<AdminProductDto>>> SearchProductsForAdmin(string searchTerm, string adminId)
        {
            var response = new ServiceResponse<List<AdminProductDto>>();

            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    response.Success = false;
                    response.Message = "Search product canot be empty";
                    return response;
                }

                searchTerm = searchTerm.ToLower().Trim();
                
                var products = await _productRepository.SearchProductsAsync(searchTerm, IncludeInactive: true);

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
                    response.Message = "Search product canot be empty";
                    return response;
                }

                searchTerm = searchTerm.ToLower().Trim();
                
                var products = await _productRepository.SearchProductsAsync(searchTerm, IncludeInactive: false);

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
            var product = await _productRepository.GetProductById(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;
                }

            product.Name = updateproductDto.Name;
            product.Description = updateproductDto.Description;
            product.PackagePrice = updateproductDto.PackagePrice;
            product.UnitPrice = updateproductDto.UnitPrice;
            product.Category = updateproductDto.Category;
            product.ContainerCount = updateproductDto.ContainerCount;
            product.ContainerType = updateproductDto.ContainerType;
            product.ContainerSize = updateproductDto.ContainerSize;
            product.PackageType = updateproductDto.PackageType;
            product.PackageSize = updateproductDto.PackageSize;
            product.Quantity = updateproductDto.Quantity;
            product.UpdatedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateProductAsync(product);

            response.Success = true;
            response.Message = "Product updated successfully";
            response.Data = MapToAdminProductDto(updatedProduct);
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
                if (newQuantity < 0)
                {
                    response.Success = false;
                    response.Message = "Quantity cannot be negative";
                    return response;
                }

                var product = await _productRepository.GetProductById(id);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Product not found";
                    return response;
                }

                var oldQty = product.Quantity;

                // update stock qty in Db
                var success = await _productRepository.UpdateProductStockAsync(id, newQuantity);
                if (!success)
                {
                    response.Success = false;
                    response.Message = "Failed to update product stock";
                    return response;
                }
                
                product.Quantity = newQuantity;
                product.UpdatedAt = DateTime.UtcNow;

                response.Success = true;
                response.Message = $"Stock updated from {oldQty} to {newQuantity}";
                response.Data = MapToAdminProductDto(product);
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating product stock: {ex.Message}";
                return response;
            }
        }

        // Mapping methods: Data transfer objects

        private static AdminProductDto MapToAdminProductDto(Product product)
        {
            return new AdminProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                PackagePrice = product.PackagePrice,
                Category = product.Category,
                PackageType = product.PackageType,
                ContainerCount = product.ContainerCount,
                ContainerSize = product.ContainerSize,
                Quantity = product.Quantity,
                IsActive = product.IsActive,
                UpdatedAt = product.UpdatedAt,
                CreatedBy = product.CreatedBy,
                CreatedAt = product.CreatedAt,

                // Properties to display
                ProductDisplayName = product.ProductDisplayName,
                UnitPriceDisplay = product.UnitPriceDisplay,
                PackagePriceDisplay = product.PackagePriceDisplay,
                FullPriceDisplay = product.FullPriceDisplay,
                ContainerTypeDisplay = product.ContainerTypeDisplay,
                ContainerDescription = product.ContainerDescription,
                CategoryDisplay = product.CategoryDisplay,
                PackageTypeDisplay = product.PackageTypeDisplay,
                IsInStock = product.IsInStock,
                StockStatusDisplay = product.StockStatusDisplay,
            };
        }

        private static ProductSummaryDto MapToProductSummaryDto(Product product)
        {
            return new ProductSummaryDto
            {
                Id = product.Id,
                Name = product.Name,
                PackagePrice = product.PackagePrice,
                PackageSize = product.PackageSize,
                ContainerCount = product.ContainerCount,
                Category = product.Category,
                MeasurementUnit = product.MeasurementUnit,
                ContainerSize = product.ContainerSize,
                IsInStock = product.IsInStock,
                ProductDisplayName = product.ProductDisplayName,
                ContainerTypeDisplay = product.ContainerTypeDisplay,
                ContainerDescription = product.ContainerDescription,
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
                PackagePrice = product.PackagePrice,
                Category = product.Category,
                PackageType = product.PackageType,
                ContainerType = product.ContainerType,
                ContainerCount = product.ContainerCount,
                MeasurementUnit = product.MeasurementUnit,
                ContainerSize = product.ContainerSize,
                UnitPrice = product.UnitPrice,
                PackageSize = product.PackageSize,
                Quantity = product.Quantity,
                IsActive = product.IsActive,

                // customer display
                ProductDisplayName = product.ProductDisplayName,
                UnitPriceDisplay = product.UnitPriceDisplay,
                PackagePriceDisplay = product.PackagePriceDisplay,
                ContainerTypeDisplay = product.ContainerTypeDisplay,
                FullPriceDisplay = product.FullPriceDisplay,
                ContainerDescription = product.ContainerDescription,
                StockStatusDisplay = product.StockStatusDisplay,
                CategoryDisplay = product.CategoryDisplay,
                IsInStock = product.IsInStock,
                MeasurementUnitDisplay = product.MeasurementUnitDisplay
            };
        }

    }
}