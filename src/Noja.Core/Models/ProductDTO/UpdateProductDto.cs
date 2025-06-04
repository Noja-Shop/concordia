using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums;

namespace Noja.Application.Models.ProductDTO
{
    public class UpdateProductDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name {get; set;}

        [StringLength(500)]
        public string Description {get; set;}

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price {get; set;}

        [Required]
        public ProductCategory Category { get; set; }

        [Required]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit size must be greater than 0")]
        public decimal UnitSize {get; set;}

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity {get; set;}

        public bool IsActive {get;set;} = true;

        public DateTime? UpdatedAt {get; set;}
    }
}