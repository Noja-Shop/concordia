using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Core.Entity
{
    public class Contribution
    {
        public Guid Id { get; set; }

        // Foreign keys
        public string CustomerId { get; set; }
        public Guid TeamId { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public Team Team { get; set; }

        public decimal Quantity { get; set; } // e.g. 10kg
        public decimal Amount { get; set; }   // e.g. Quantity * UnitPrice
        public DateTime CreatedAt { get; set; }
        bool IsCreator { get; set; } // Indicates if the contribution is made by the team creator
        public Guid? PaymentId { get; set; }
        public Payment? Payment { get; set; }
    }
}