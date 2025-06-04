using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Core.Models.ProductDTO
{
    public class UpdateStockDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]

        public int Quantity { get; set; }
    }
}