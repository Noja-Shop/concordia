using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Enums.Team;

namespace Noja.Core.Entity
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(10,3)")]
        public decimal TargetQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // ===== Team Ownership ==== //
        [Required]
        public string CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public Customer Creator { get; set; }

        public TeamStatus Status { get; set; } = TeamStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Participants (including Creator)
        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();

        // ===== Calculated Properties ===== //

        [NotMapped]
        public int CurrentParticipants => Members?.Count ?? 0;

        [NotMapped]
        public decimal TotalCommitted => Members?.Sum(m => m.AmountPaid) ?? 0;

        [NotMapped]
        public decimal TotalPaid => Members?
        .Where(m => m.Payment.Status == PaymentStatus.Completed)
        .Sum(m => m.AmountPaid) ?? 0;

        [NotMapped]
        public decimal RemainingQuantity => TargetQuantity - TotalCommitted;

        [NotMapped]
        public decimal RemainingAmount => TargetAmount - TotalPaid;

        [NotMapped]
        public bool IsTargetReached => TotalPaid >= TargetAmount;

        [NotMapped]
        public bool CanJoin => Status == TeamStatus.Active && !IsTargetReached;

        [NotMapped]
        public decimal ProgressPercentage => TargetAmount > 0 ? TotalPaid / TargetAmount * 100 : 0;

        // ===== Display Properties ===== //
        [NotMapped]
        public string ProgressDisplay => GetProgressDisplay();

        private string GetProgressDisplay()
        {
            if (Product == null) return "";

            var unit = Product.MeasurementUnitDisplay;
            var quantityProgress = $"{TotalCommitted:N2} {unit} of {TargetQuantity:N2} {unit}";
            var amountProgress = $"₦{TotalPaid:N2} of ₦{TargetAmount:N2}";

            return $"{quantityProgress} | {amountProgress} ({ProgressPercentage:F1}%)";
        }

        [NotMapped]
        public string StatusDisplay => Status switch
        {
            TeamStatus.Active => IsTargetReached ? "Completed" : "Active",
            TeamStatus.Completed => "Completed",
            TeamStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };
    }
}