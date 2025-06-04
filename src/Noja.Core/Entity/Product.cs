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

        [Required]
        [Column(TypeName = "decimal(18,2)" )]
        public decimal Price {get; set;} 
        // price per unit (per kg,per piece etc)

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitSize {get; set;} 
        // 50 for 50kg bag, 1 for 1 piece

        [Required]
        public int Quantity {get; set;} // Available stock in number
        public bool IsActive {get; set;}

        [Required]
        public ProductCategory Category {get; set;}

        [Required]
        public UnitOfMeasure UnitOfMeasure {get; set;}

        public string CreatedBy {get;set;}

        [ForeignKey("CreatedBy")]
        public NojaUser CreatedByUser {get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }

        //===== Display properties =======//

        [NotMapped]
        public string UnitAbbreviation => GetUnitAbbreviation();

        //<Summary>
        // Gets the package type name (bag, bottle, pack, etc)

        private string GetUnitAbbreviation()
        {
            return UnitOfMeasure switch
            {
                UnitOfMeasure.Kilogram => "Kg",
                UnitOfMeasure.Piece => "pc",
                UnitOfMeasure.Bag => "bag",
                UnitOfMeasure.Box => "box",
                _ => "unit"
            };
        }

        [NotMapped]
        public string PackageType => GetPackageType();

        //<Summary>
        // Gets total price for one complete paackage

        private string GetPackageType()
        {
           return UnitOfMeasure switch
            {
                UnitOfMeasure.Kilogram => "Kg",
                UnitOfMeasure.Piece => "pc",
                UnitOfMeasure.Bag => "bag",
                UnitOfMeasure.Box => "box",
                _ => "package"
            };
        }

        [NotMapped]
        public string DisplayName => GetDisplayName();

        // <summary>
        // Gets unit price display text (e.g "N125.00 for 50kg bag)

        private string GetDisplayName()
        {
            if (UnitSize == 1 && (UnitOfMeasure == UnitOfMeasure.Piece || UnitOfMeasure == 
            UnitOfMeasure.Bag || UnitOfMeasure == UnitOfMeasure.Box || UnitOfMeasure == UnitOfMeasure.Kilogram
            
            ))
            {
                return Name;
            }

            return $"{Name} - {UnitSize}{UnitAbbreviation}{PackageType}"; 
        }


        public string UnitPriceDisplay => GetUnitPriceDisplay();

        // <Summary>
        // Get package price display text (e.g "N152.00 for 50kg bag")

        private string GetUnitPriceDisplay()
        {
            var currency = "N"; 
            return UnitOfMeasure switch
            {
                UnitOfMeasure.Kilogram => $"{currency}{Price:F2} per Kg",
                UnitOfMeasure.Piece => $"{currency}{Price:F2} per piece",
                UnitOfMeasure.Bag => $"{currency}{Price:F2} per bag",
                UnitOfMeasure.Box => $"{currency}{Price:F2} per box",
                _ => $"{currency}{Price:F2} per unit"
            };
        }

        [NotMapped]
        public string PackagePriceDisplay => GetPackagePriceDisplay();

        // <Summary>
        // Gets complete price display for customers

        private string GetPackagePriceDisplay()
        {
            var currency = "N"; 
            return UnitOfMeasure switch
            {
                UnitOfMeasure.Kilogram => $"{currency}{Price:F2} per Kg",
                UnitOfMeasure.Piece => $"{currency}{Price:F2} per piece",
                UnitOfMeasure.Bag => $"{currency}{Price:F2} per bag",
                UnitOfMeasure.Box => $"{currency}{Price:F2} per box",
                _ => $"{currency}{Price:F2} per unit"
            };
        }

        [NotMapped]
        public string FullPriceDisplay => GetFullPriceDisplay();

        // <Summary>
        // Checks if product is currently in stock
        
        private string GetFullPriceDisplay()
        {
            if (UnitSize == 1)
            {
                return UnitPriceDisplay;
            }

            return $"{UnitPriceDisplay}({PackagePriceDisplay})";
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
            
            if (Quantity <= 5)
                return $"Low stock ({Quantity} {(Quantity == 1 ? PackageType : PackageType + "s")} left)";
            
            return $"In stock ({Quantity} {(Quantity == 1 ? PackageType : PackageType + "s")} available)";

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

        // ======= Utility Methods ======= //
        // <SUMMARY>
        // Calculate total price for a given QTY
        
        public decimal CalculatePriceForQuantity(decimal requestedQTY)
        {
            return Price * requestedQTY;
        }

        [NotMapped]
        public bool IsInStock => Quantity > 0 && IsActive;

        // <Summary>
        // Check if requested qty is available

        public bool IsQTYAvailable(decimal requestedQTY)
        {
            if (!IsInStock) return false;
            var requiredPackages = Math.Ceiling(requestedQTY/UnitSize);
            return requiredPackages <= Quantity;
        }



    }

    
}