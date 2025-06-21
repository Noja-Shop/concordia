using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.ProductDTO;

namespace Noja.Core.Interfaces.Service
{
        // <summary>
        // Application service interface for product business operations.
        // Provides high-level business logic for product management across different user roles.
        // All methods return ServiceResponse wrapper for consistent error handling and API responses.
        // </summary>
        // <remark>
        // It handles:
        // - Validation
        // - DTO mapping and transaformation
        // - Rolebased data filtering (Admin vs Customer views)
        // </remark>
    public interface IProductService
    {
        
        // <summary>
        // Creates a new product in the system (Admin only)
        // </summary>
        // <param name = "CreateProductDto"> Product creation data transfer object containing all 
        // required fields for product creation.
        // </param name = "ID"> ID of the admin user creating the product
        // <returns>
        // ServiceResponse containing: 
        // -Success: AdminProductDto with complete productdetails including admin ID
        // -Failure: Erros msg with validation details or system error information
        // </returns>
        Task<ServiceResponse<AdminProductDto>> CreateProductAsync(CreateProductDto createproductDto, string adminId);

        
        Task<ServiceResponse<AdminProductDto>> UpdateProductAsync(Guid id, UpdateProductDto updateproductDto, string adminId);
        Task<ServiceResponse<AdminProductDto>> DeleteProductAsync(Guid id, string adminId);
        Task<ServiceResponse<AdminProductDto>> UpdateProductStockAsync(Guid id, int newQuantity, string adminId);
        Task<ServiceResponse<AdminProductDto>> GetProductForAdminAsync(Guid id, string adminId);
        Task<ServiceResponse<ProductDto>> GetProductDetailsForCustomerAsync(Guid id);
        Task<ServiceResponse<List<ProductDto>>> GetAllProductsAsync();
        Task<ServiceResponse<List<ProductDto>>> GetActiveProductsAsync();
        Task<ServiceResponse<List<ProductDto>>> GetProductsByCategoryAsync(string category);
        Task<ServiceResponse<List<ProductSummaryDto>>> SearchProductsForCustomersAsync(string searchTerm);
        Task<ServiceResponse<List<AdminProductDto>>> SearchProductsForAdmin(string searchTerm, string adminId);

        // <summary>
        // Gets all active products with summary information for listings
        // </summary>
        // Task<ServiceResponse<List<ProductSummaryDto>>> GetActiveProductsSummaryAsync();

    }
}