using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Entity;
using Noja.Core.Enums;

namespace Noja.Core.Interfaces.Repository
{

    // <summary>
    // Repository interface for Product data access operations
    // </summary>

    public interface IProductRepository
    {
        // ====== CRUD ====== //

        // <summary>
        // Gets a product by its unique Guid Id
        // <param name> = "id">Product unique id </param>
        // <returns>Product entity if found, null otherwise</returns>
        // </summary>
        Task<Product?> GetProductById(Guid id);

        // <summary>
        // Adds/Create a new product to the repository
        // <param name = "product"> Product entity to add </param>
        // <returns> A new product with a generated ID </param>
        // </summary>
        Task<Product> CreateProductAsync(Product product);

        // <summary>
        // Updates existing values of product
        // <param name = "product"> Product entity with updated values </param>
        // <returns> Updated product </param>
        // </summary>
        Task<Product> UpdateProductAsync(Product product);

        // <summary>
        // Decreases the Quantity of a product whe a Team is created
        // <param name = "productId"> Product ID to update </param>
        // <param name = "newQuantity"> New stock quantity </param>
        // <returns> Updated product with new quantity </returns>
        // </summary>
        Task<Product> UpdateQuantityAsync(Guid productId, int newQuantity);

        // <summary>
        // Soft deletes a product(sets IsActive = false)
        // <param name = "id"> Product ID to delete </param>
        // <returns>True if delete is success</return>
        // </summary>
        Task<bool> DeleteProductAsync(Guid id);


        // ========== Query Ops ============//


        // <summary>
        // Gets all products, includes inactive - Admin view
        // <returns>List of products</returns>
        // </summary>
        Task<List<Product>> GetAllProductsAsync();

        // <summary>
        // Gets all active products, excludes inactive - Customer view
        // <returns>List of products in stock</returns>
        // </summary>
        Task<List<Product>> GetActiveProductsAsync();

        // <summary>
        // Get products by category
        // <param name = "category"> Category to filter by </param>
        // <param name = "IncludeInactive"> If to include active products </param>
        // <returns>List of products specified by category </rteurns>
        // </summary>
        Task<List<Product>> GetByCategoryAsync(ProductCategory category, bool IncludeInactive = false);
        

        // ======  Search Ops =======//

        // <summary>
        // Search products by name, description or category 
        // <param name = "searchTerm"> Search term to match </param>
        // <param name = "IncludeInactive"> If to include inactive products
        // <returns>List of products matching search term</returns>
        // </summary>
        Task<List<Product>> SearchProductsAsync(string searchTerm, bool IncludeInactive = false);

        // ======= Stock Mgmt ======= //
        // <summary>
        // Updates the quantity of a product
        // <param name = "productId"> Product ID to update </param>
        // <param name = "newQuantity"> New stock quantity </param>
        // <returns>True if update is success</returns>
        // </summary>
        Task<bool> UpdateProductStockAsync(Guid productId, int newQuantity);

        // ============== Utility Ops ========//

        // <summary>
        // Checks if a product exists by its ID
        // <param name = "id"> Product ID to check </param>
        // <returns>True if product exists</returns>
        // </summary>
        Task<bool> ProductExistsAsync(Guid id);

        // <summary>
        // Gets the total count of products
        // <param name="includeInactive">If to include inactive products in count</param>
        // <returns> Total number of products </returns>
        // </summary>
        Task<int> GetProductCountAsync(bool includInactive = false);




    }
}