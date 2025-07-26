using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums.Team;

namespace Noja.Core.Entity
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(100)]
        public string TransactionReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Simulation fields
        public bool SimulateSuccess { get; set; } = true;
        public int SimulationDelaySeconds { get; set; } = 2;

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public Contribution Contribution { get; set; }

        // Display properties
        [NotMapped]
        public string StatusDisplay => Status switch
        {
            PaymentStatus.Pending => "⏳ Pending",
            PaymentStatus.Processing => "🔄 Processing",
            PaymentStatus.Completed => "✅ Paid",
            PaymentStatus.Failed => "❌ Failed",
            _ => Status.ToString()
        };

    }
}