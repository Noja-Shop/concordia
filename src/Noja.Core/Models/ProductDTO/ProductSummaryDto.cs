using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums;

namespace Noja.Application.Models.ProductDTO
{
    public class ProductSummaryDto
    {
        // For lists of products
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ProductCategory Category { get; set; }
        public bool IsInStock { get; set; }
        
        // Essential display info for product cards
        public string DisplayName { get; set; }
        public string UnitPriceDisplay { get; set; }
        public string StockStatusDisplay { get; set; }
        public decimal PackagePrice { get; set; }
    }
}