using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums.Team;

namespace Noja.Core.Models.TeamDTO
{
    public class PaymentDto
    {
        public Guid id { get; set; }
        public string CustomerId { get; set; }
        public Guid TeamId { get; set; }
        public string CustomerName { get; set; }
        public string TeamName { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool SimulateSuccess {get; set;}
        public string StatusDisplay { get; set; }

    }
}