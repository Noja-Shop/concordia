using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Core.Entity
{
    public class TeamMember
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(10,3)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [Required]
        public Guid PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Helper properties
        [NotMapped]
        public bool IsCreator => Team?.CreatedBy == CustomerId;

        // ===== Display properties =====//

        [NotMapped]
        public string CommitmentDisplay => GetCommitmentDisplay();
        
        [NotMapped]
        public string RoleDisplay => IsCreator ? "👑 Creator" : "👤 Member";

        private string GetCommitmentDisplay()
        {
            var unit = Team?.Product?.MeasurementUnitDisplay ?? "unit";
            var paymentStatus = Payment?.StatusDisplay ?? "Pending";
            return $"{Quantity:G29}{unit} for ₦{AmountPaid:N2} ({paymentStatus})";
        }

    }
}