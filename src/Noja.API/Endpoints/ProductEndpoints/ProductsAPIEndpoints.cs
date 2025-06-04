using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.API.Endpoints.ProductEndpoints
{
    public static class ProductsAPIEndpoints
    {
        private const string ApiBase = "api";
        public static class Product
        {
            private const string Base = $"{ApiBase}/product";
            public const string CreateProduct = $"{Base}/create";
            public const string UpdateProduct = $"{Base}/update";
            public const string DeleteProduct = $"{Base}/delete";
            public const string GetAllProducts = $"{Base}/all";
            public const string GetProductById = $"{Base}/get";
            public const string UpdateProductStock = $"{Base}/stock";
            public const string GetProductByCategory = $"{Base}/category";
            public const string GetProductForAdminBySearch = $"{Base}/admin/search";
            
        }
    }
}