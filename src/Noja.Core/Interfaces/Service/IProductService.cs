using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.ProductDTO;

namespace Noja.Core.Interfaces.Service
{
    public interface IProductService
    {
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

    }
}