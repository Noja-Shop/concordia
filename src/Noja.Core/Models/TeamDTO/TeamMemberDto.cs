using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Core.Models.TeamDTO
{
    public class TeamMemberDto
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string CustomerId { get; set; }

        // payment info
        public Guid PaymentId { get; set; }
        public decimal AmountPaid { get; set; }
        public string CommitmentDisplay {get; set;}

        // product qty
        public decimal Quantity { get; set; }
    }
}