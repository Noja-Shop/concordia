using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Entity;
using Noja.Core.Enums;
using Noja.Core.Interfaces.Repository;

namespace Noja.Application.Services.Products
{
    public class ProductService2 : IProductRepository
    {
        public Task<Product> CreateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetActiveProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetByCategoryAsync(ProductCategory category, bool IncludeInactive = false)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetProducCountAsync(bool includInactive = false)
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetProductById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ProductExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> SearchProductsAsync(string searchTerm, bool IncludeInactive = false)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductStockAsync(Guid productId, int newQuantity)
        {
            throw new NotImplementedException();
        }
    }
}