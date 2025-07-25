using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums.Team;

namespace Noja.Core.Models.TeamDTO
{
    public class CreateTeamDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target quantity must be greater than 0")]
        public decimal TargetQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Creator quantity must be greater than 0")]
        public decimal CreatorQuantity { get; set; }

        // [Range(2, 50, ErrorMessage = "Minimum participants must be between 2 and 50")]
        // public int MinParticipants { get; set; } = 2;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public bool SimulatePaymentSuccess { get; set; } = true;

    }
}