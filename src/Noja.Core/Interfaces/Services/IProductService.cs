using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.ProductDTO;

namespace Noja.Core.Interfaces.Services
{
    public interface IProductService
    {
        // ===== Admin Opertaions ===== //

        // <summary>
        // Creates a new product (AdminOnly)
        // Params createproductdto; Product creation data. adminId; Admin id creating
        // </summary>
        Task<ServiceResponse<AdminProductDto>> CreateProductAsync(CreateProductDto createproductDto, string adminId);

        // <summary>
        // Update existing product (AdminOnly)
        // Params updateproductdto; Product creation data. adminId; Admin id updating
        // Guid id; the product Id
        // </summary>
        Task<ServiceResponse<AdminProductDto>> UpdateProductAsync(Guid id, UpdateProductDto updateproductDto, string adminId);

        // <summary>
        // Delete existing product (AdminOnly)
        // Params id; the product Id. admin Id; admin deleting a product
        // </summary>
        Task<ServiceResponse<string>> DeleteProductAsync(Guid id, string adminId);

        // <summary>
        // Update product stock qty
        // Params id; the product Id. admin Id; admin updating a product,
        // newQuantity; New stock qty
        // </summary>
        Task<ServiceResponse<AdminProductDto>> UpdateProductStockAsync(Guid id, int newQuantity, string adminId);

        // <summary>
        // Gets all  products for admin dashboard (includes inactive)
        // Params: adminId; the admin requesting the product
        // </summary
        Task<ServiceResponse<List<AdminProductDto>>> GetAllProductsForAdminAsync(string adminId);

        // <summary>
        // Gets a specific product requested by admin.
        // Params: Id - product id || the adminId - admin requesting a specific product
        // </summary>
        Task<ServiceResponse<AdminProductDto>> GetProductForAdminAsync(Guid id, string adminId);

        // <summary>
        // Searches products for admin (include inactive products)
        // Params: searchTerm - string of product being searched
        // Params: adminId - the admin searching for the products
        // </summary>
        Task<ServiceResponse<List<AdminProductDto>>>  SearchProductsForAdmin(string searchTerm, string adminId);


        // ========== Customer Operations (Read only) ========//

        // <summary>
        // Gets all active products for customer browsing
        // </summary>
        Task<ServiceResponse<List<ProductSummaryDto>>> GetActiveProductsForCustomerAsync();

        // <summary>
        // Gets a specific product for customer browsing
        // returns detailed product information
        // </summary>
        Task<ServiceResponse<ProductDto>> GetProductDetailsForCustomerAsync(Guid id);

        // <summary>
        // Searches active products for customers
        // Params: searchTerm - string of product being searched
        // </summary>
        Task<ServiceResponse<List<ProductSummaryDto>>> SearchProductsForCustomersAsync(string searchTerm);







    }
}