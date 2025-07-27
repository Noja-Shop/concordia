using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Core.Models.TeamDTO
{
    public class CreateContributionDto
    {
        public Guid TeamId { get; set; }
        public string CustomerId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public Guid? PaymentId { get; set; }
    }
}