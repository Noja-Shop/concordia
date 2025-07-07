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
        [Range(0.01, double.MaxValue, ErrorMessage = "PackagePrice must be greater than 0")]
        public decimal PackagePrice {get; set;}

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "PackageSize must be greater than 0")]
        public decimal PackageSize { get; set; }

        [Required]
        public ProductCategory Category { get; set; }

        [Required]
        public MeasurementUnit MeasurementUnit{ get; set; }

        [Required]
        public PackageType PackageType { get; set; }

        [Required]
        public int ContainerCount {get; set;}

        [Required]
        public ContainerType ContainerType {get; set;}

        [Required]
        public decimal ContainerSize {get; set;}

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity {get; set;}

        public bool IsActive {get;set;} = true;

        public DateTime? UpdatedAt {get; set;}
    }
}