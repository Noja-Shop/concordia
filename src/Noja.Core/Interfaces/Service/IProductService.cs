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
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        Task<ServiceResponse<AdminProductDto>> CreateProductAsync(CreateProductDto createproductDto, string adminId);

        // <summary>
        // Updates an exisiting product (Admin only)
        // <param name = "UpdateProductDto"> Product update data transfer object contining all the required fields for updating/editing a product
        // <param name = "ID"> Guid id for the specific product to be updated>
        // <returns>
        // ServiceResponse containing
        // -Success: UpdateProductDto with complete productdetails including admin ID
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<AdminProductDto>> UpdateProductAsync(Guid id, UpdateProductDto updateproductDto, string adminId);

        // <summary>
        // Remove an exisiting product (Admin only)
        // <param name = "ID"> Guid id for the specific product to be deleted>
        // <returns>
        // ServiceResponse containing
        // -Success: Returns a succeeful msg for the deleted product
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<AdminProductDto>> DeleteProductAsync(Guid id, string adminId);

        // <summary>
        // Updates an exisiting product qunatity(stock) (Admin only)
        // <param name = "newQuantity"> Product update data transfer object contining all the required fields for updating/editing a product
        // <param name = "ID"> Guid id for the specific product quantity field to be updated>
        // <returns>
        // ServiceResponse containing
        // -Success: quantity with new or updated quantity (shows the old -- new)
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<AdminProductDto>> UpdateProductStockAsync(Guid id, int newQuantity, string adminId);

        // <summary>
        // Retrives a specific product (Admin only)
        // <param name = "ID"> Guid id for the specific product to be retrieved>
        // <returns>
        // ServiceResponse containing
        // -Success: A product with its details
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<AdminProductDto>> GetProductForAdminAsync(Guid id, string adminId);

        // <summary>
        // Retrieve a specific product to the customer
        // <param name = "ID"> Guid id for the specific product to be retrieved>
        // <returns>
        // ServiceResponse containing
        // -Success: Retrieved product along with its details/description
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<ProductDto>> GetProductDetailsForCustomerAsync(Guid id);

        // <summary>
        // All products are retrieved even if out of stock
        // ServiceResponse containing
        // -Success: JSON of existing products
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<List<ProductDto>>> GetAllProductsAsync();

        // <summary>
        // Retrieves Products that a customer can buy ie quantity is > 0
        // <returns>
        // ServiceResponse containing
        // -Success: JSON of all products where quantity > 0
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<List<ProductDto>>> GetActiveProductsAsync();

        // <summary>
        // Retrieves a products based on a specific category [Fruits Category]
        // <param name = "category"> A specific group/label of products>
        // <returns>
        // ServiceResponse containing
        // -Success: Returns all products associated with a particular category
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<List<ProductDto>>> GetProductsByCategoryAsync(string category);

        // <summary>
        // A query parameter for a product can be specific or general
        // <param name = "searchTerm"> The name of the product/description/category>
        // <returns>
        // ServiceResponse containing
        // -Success: A product or products based on the searchTerm
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<List<ProductSummaryDto>>> SearchProductsForCustomersAsync(string searchTerm);

        // <summary>
        // A query parameter for a product can be specific or general(admin only)
        // <param name = "searchTerm"> The name of the product/description/category>
        // <returns>
        // ServiceResponse containing
        // -Success: A product or products based on the searchTerm
        // -Failure: Errors msg with validation details or system error information
        // </returns>
        // </summary>
        Task<ServiceResponse<List<AdminProductDto>>> SearchProductsForAdmin(string searchTerm, string adminId);

    }
}