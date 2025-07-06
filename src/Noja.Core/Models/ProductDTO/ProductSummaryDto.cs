using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums;

namespace Noja.Application.Models.ProductDTO
{
    public class ProductSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        // ===== Pricing  ===== //
        public decimal PackagePrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PackageSize { get; set; }
        
        // ===== Product Classification ===== //
        public ProductCategory Category { get; set; }
        public MeasurementUnit MeasurementUnit { get; set; }
        public PackageType PackageType { get; set; }
        
        // ===== Stock Status ===== //
        public bool IsInStock { get; set; }
        public bool IsActive { get; set; }
        public int Quantity { get; set; } // Useful for "Only X left" messages
        
        // ===== Essential Display Properties for Product Cards ===== //
        public string ProductDisplayName { get; set; }
        public string PackagePriceDisplay { get; set; }
        public string UnitPriceDisplay { get; set; }
        public string FullPriceDisplay { get; set; }
        public string StockStatusDisplay { get; set; }
        public string CategoryDisplay { get; set; }
        public string PackageTypeDisplay {get; set;}
        
        /// <summary>
        /// For product card badges (e.g., "50kg", "1.5L", "Single")
        /// </summary>
        public string PackageSizeDisplay { get; set; }
    }
}