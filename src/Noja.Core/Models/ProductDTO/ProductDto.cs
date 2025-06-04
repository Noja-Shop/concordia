using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums;

namespace Noja.Application.Models.ProductDTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ProductCategory Category { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public decimal UnitSize { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Display properties for customers
        public string DisplayName { get; set; }
        public string UnitPriceDisplay { get; set; }
        public string PackagePriceDisplay { get; set; }
        public string FullPriceDisplay { get; set; }
        public string StockStatusDisplay { get; set; }
        public string CategoryDisplay { get; set; }
        public bool IsInStock { get; set; }
        public decimal PackagePrice { get; set; }
        public string UnitAbbreviation { get; set; }
    }
}