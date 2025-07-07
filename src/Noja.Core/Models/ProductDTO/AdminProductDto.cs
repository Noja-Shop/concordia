using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums;

namespace Noja.Application.Models.ProductDTO
{
    public class AdminProductDto
    {
        // For the Admin dashboard
        
        // ===== Core Properties ===== //
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// Full price for the complete package - what customer pays
        /// </summary>
        public decimal PackagePrice { get; set; }
        
        /// <summary>
        /// Price per single unit - for customer comparison
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Size of one package (e.g., 50 for 50kg bag)
        /// </summary>
        public decimal PackageSize { get; set; }
        public int ContainerCount {get; set;}
        public decimal ContainerSize {get; set;}
        public ContainerType ContainerType {get; set;}
        public ProductCategory Category { get; set; }
        public MeasurementUnit MeasurementUnit { get; set; }
        public PackageType PackageType { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        
        // ===== Audit Properties ===== //
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        
        // ===== Display Properties ===== //
        public string ProductDisplayName { get; set; }
        public string PackagePriceDisplay { get; set; }
        public string UnitPriceDisplay { get; set; }
        public string FullPriceDisplay { get; set; }
        public string ContainerTypeDisplay {get; set;}
        public string ContainerDescription {get; set;}
        public string StockStatusDisplay { get; set; }
        public string CategoryDisplay { get; set; }
        public string MeasurementUnitDisplay { get; set; }
        public string PackageTypeDisplay { get; set; }
        
        // ===== Calculated Properties ===== //
        public bool IsInStock { get; set; }
        public bool IsSingleUnit { get; set; }
    }
}