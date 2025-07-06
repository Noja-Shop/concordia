using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums;

namespace Noja.Core.Entity
{
    public class Product
    {
        [Key]
        public Guid Id {get;set;} = Guid.NewGuid();
        
        [Required]
        [MaxLength(100)]
        public string Name {get; set;}

        [Required]
        [MaxLength(500)]
        public string Description {get; set;}

        // <summary>
        // Full price of the package. eg, N4,800 for 50kg bag of rice
        // </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Package price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)" )]
        public decimal PackagePrice {get; set;} 
        
        // <summary>
        // Price of a single unit of the product. eg, N100 for 50kg bag of rice
        // e.g, N100 per kg
        // </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice {get; set;}

        //<summary>
        // Size of one package. eg, 50kg bag of rice
        // </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Package size must be greater than 0")]
        [Column(TypeName = "decimal(10,3)")]
        public decimal PackageSize { get; set; }
       
       [Required]
       public MeasurementUnit MeasurementUnit {get; set;}

       [Required]
       public PackageType PackageType {get; set;}

        [Required]
        public int Quantity {get; set;} // Available stock in number
        public bool IsActive {get; set;}

        [Required]
        public ProductCategory Category {get; set;}

        public string CreatedBy {get;set;}

        [ForeignKey("CreatedBy")]
        public NojaUser CreatedByUser {get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }

        // ======== Contants =======//
        private const string CURRENCY_SYMBOL = "₦";

        // ==========Calulated Properties =========//

        [NotMapped]
        public bool IsInStock => Quantity > 0 && IsActive;

        [NotMapped]
        public bool IsSingleUnit => PackageSize == 1;


        //===== Display properties =======//

        [NotMapped]
        public string MeasurementUnitDisplay => GetMeasurementUnitDisplay();

        private string GetMeasurementUnitDisplay()
        {
            return MeasurementUnit switch
            {
                MeasurementUnit.Kilogram => "kg",
                MeasurementUnit.Liter => "L",
                MeasurementUnit.Piece => "piece",
                _ => "unit"
            };
        }

        [NotMapped]
        public string PackageTypeDisplay => GetPackageTypeDisplay();
        
        private string GetPackageTypeDisplay()
        {
            return PackageType switch
            {
                PackageType.Bag => "bag",
                PackageType.Box => "box",
                PackageType.Can => "can",
                PackageType.Sack => "sack",
                PackageType.Carton => "carton",
                _ => "package"
            };
        }

        [NotMapped]
        public string ProductDisplayName => GetProductDisplayName();

        private string GetProductDisplayName()
        {
            if (IsSingleUnit)
                return Name;
            
            return $"{Name} - {PackageSize:G29}{MeasurementUnitDisplay} {PackageTypeDisplay}";
        }
        
        // <summary>
        // Shows the package full price (e.g "N152.00 for 50kg bag")
        // </summary>
        [NotMapped]
        public string PackagePriceDisplay => $"{CURRENCY_SYMBOL}{PackagePrice:N2}";

        // <summary>
        // Shows the unit price (e.g "N152.00 for 50kg bag")
        // </summary>
        [NotMapped]
        public string UnitPriceDisplay => $"{CURRENCY_SYMBOL}{UnitPrice:F2} per {MeasurementUnitDisplay}";

        // <Summary>
        // Get package price display text (e.g "N152.00 for 50kg bag")

        [NotMapped]
        public string FullPriceDisplay => GetFullPriceDisplay();
        private string GetFullPriceDisplay()
        {
           if(IsSingleUnit) return PackagePriceDisplay;
           return $"{PackagePriceDisplay} ({UnitPriceDisplay})";
        }

        [NotMapped]
        public string StockStatusDisplay => GetStockStatusDisplay();

        // <Summary>
        // Gets the stock active/inactive

        private string GetStockStatusDisplay()
        {
            if (!IsActive)
             return "Inactive";

            if (Quantity == 0)
             return "Out of stock";
            
            var packageWord = Quantity == 1 ? PackageTypeDisplay : GetPluralPackaging();
            if (Quantity <= 5)
                return $"Low stock ({Quantity} {packageWord} available)";
            
            return $"In stock ({Quantity} {packageWord} left)";

        }

        private string GetPluralPackaging()
        {
            return PackageType switch
            {
                PackageType.Bag => "bags",
                PackageType.Box => "boxes",
                PackageType.Can => "cans",
                PackageType.Sack => "sacks",
                PackageType.Carton => "cartons",
                _ => PackageTypeDisplay + "s"
            };
        }

        [NotMapped]
        public string CategoryDisplay => GetCategoryDisplay();

        // <Summary>
        // Gets the category display name

        private string GetCategoryDisplay()
        {
            return Category switch
            {
                ProductCategory.Dairy => "Dairy Products",
                ProductCategory.Grains => "Grains & Cereals",
                ProductCategory.Fruits => "Fresh Fruits",
                ProductCategory.Others => "Others",
                ProductCategory.Vegetables => "Fresh Vegs",
                ProductCategory.Meat => "Meat",
                _ => Category.ToString()
            };
        }

    }

    
}