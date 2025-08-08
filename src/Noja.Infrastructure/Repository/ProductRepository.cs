using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Noja.Core.Entity;
using Noja.Core.Enums;
using Noja.Core.Interfaces.Repository;
using Noja.Infrastructure.Data;

namespace Noja.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly NojaDbContext _context;

        public ProductRepository(NojaDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductById(Guid id)
        {
            return await _context.Products
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateQuantityAsync(Guid productId, int newQuantity)
        {
            var product = await _context.Products
            .FromSqlRaw("SELECT * FROM \"Products\"WHERE \"Id\" = {0} FOR UPDATE", productId)
            .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found");
            }

            product.Quantity = newQuantity;

            // mark the Quantity property to be modified
            _context.Entry(product).Property(q => q.Quantity).IsModified = true;

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

        public Task<bool> DeleteProductAsync(Guid id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.CreatedBy)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive && p.Quantity > 0)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryAsync(ProductCategory category, bool includeInactive = false)
        {   
            var query =  _context.Products
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name).AsQueryable();   

            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive && p.Quantity > 0);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm, bool includeInactive = false)
        {
            searchTerm = searchTerm.ToLower().Trim();
            var searchWords = searchTerm.Split(' ',StringSplitOptions.RemoveEmptyEntries);

            var query =  _context.Products.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive && p.Quantity > 0);
            }

            query = query.Where(p => searchWords.Any(word => p.Name.ToLower().Contains(word) || 
            p.Description.ToLower().Contains(word) ||
            p.Category.ToString().ToLower().Contains(word)
            ));

            query = query.OrderBy(p => p.Name.ToLower().StartsWith(searchTerm) ? 0 : 1)
            .ThenBy(p => p.Name);
            return await query.ToListAsync();

        }

        public async Task<bool> UpdateProductStockAsync(Guid productId, int newQuantity)
        {
            var productStock = await _context.Products.FindAsync(productId);
            if (productStock == null) return false;

            productStock.Quantity = newQuantity;
            productStock.UpdatedAt = DateTime.UtcNow;

           await _context.SaveChangesAsync();

           return true;
        }

        public async Task<bool> ProductExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
            
        }

        public async Task<int> GetProductCountAsync(bool includeInactive = false)
        {
            if (!includeInactive)
            {
                return await _context.Products
                .Where(p => p.IsActive && p.Quantity > 0)
                .CountAsync();
            }

            return await _context.Products.CountAsync();

        }
    }
}