using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums.Team;

namespace Noja.Core.Models.TeamDTO
{
    public class JoinTeamDto
    {
        [Required]
        public Guid TeamId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Qty must be greater than 0")]
        public decimal Quntity { get; set; }

        [Required]
        public PaymentMethod PaymentMethod {get; set;}

        public bool SimulatedPaymentSuccess { get; set; } = true;
    }
}