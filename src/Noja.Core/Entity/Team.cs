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

        [Required]
        public DateTime ExpiresAt { get; set; }

        // [Range(2, 50)]
        // public int MinParticipants { get; set; } = 2;

        // Participants (including Creator)
        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();

        // ===== Contants ===== //
        private const int EXPIRY_HOURS = 72;

        // ===== success criteria ====== //

        [NotMapped]
        public bool IsQuantityTargetReached => TotalCommitted == TargetQuantity;

        [NotMapped]
        public bool IsAmountTargetReached => TotalPaid == TargetAmount;

        // [NotMapped]
        // public bool HasMinParticipants => CurrentParticipants >= MinParticipants;

        [NotMapped]
        public bool IsSuccessful => IsQuantityTargetReached && IsAmountTargetReached;

        // ===== Expiry properties =======//

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;

        [NotMapped]
        public TimeSpan TimeRemaining => ExpiresAt > DateTime.UtcNow ? ExpiresAt - DateTime.UtcNow : TimeSpan.Zero;

        [NotMapped]
        public bool IsExpiringSoon => TimeRemaining.TotalHours <= 6;

        [NotMapped]
        public bool IsCritical => TimeRemaining.TotalHours <= 2;

        // ===== Calculated Properties ===== //

        [NotMapped]
        public int CurrentParticipants => Members?.Count ?? 0;

        [NotMapped]
        public decimal TotalCommitted => Members?.Sum(m => m.Quantity) ?? 0;

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

        [NotMapped]
        public decimal QuantityProgressPercentage => TargetQuantity > 0 ? TotalCommitted / TargetQuantity * 100 : 0;
        

        // ===== BUSINESS METHODS ===== //
        /// <summary>
        /// Initialize team with automatic 72-hour expiry
        /// </summary>
        public void InitializeExpiry()
        {
            ExpiresAt = CreatedAt.AddHours(EXPIRY_HOURS);
        }
        

        public bool CanMemberJoin(decimal requestedQuantity, out string reason)
        {
            reason = "";
            
            if (IsExpired)
            {
                reason = "Team has expired";
                return false;
            }
            
            if (Status != TeamStatus.Active)
            {
                reason = "Team is not active";
                return false;
            }

            if (requestedQuantity <= 0)
            {
                reason = "Quantity must be greater than 0";
                return false;
            }

            if (TotalCommitted + requestedQuantity > TargetQuantity)
            {
                var remaining = RemainingQuantity;
                var unit = Product?.MeasurementUnitDisplay ?? "unit";
                reason = $"Only {remaining:G29}{unit} remaining";
                return false;
            }

            return true;
        }

        public void CheckAndUpdateStatus()
        {
            if (IsExpired && Status == TeamStatus.Active)
            {
                if (IsSuccessful)
                {
                    Status = TeamStatus.Completed;
                    CompletedAt = DateTime.UtcNow;
                }
                else
                {
                    Status = TeamStatus.Expired;
                }
            }
            else if (IsSuccessful && Status == TeamStatus.Active)
            {
                Status = TeamStatus.Completed;
                CompletedAt = DateTime.UtcNow;
            }
        }

        // ===== Display Properties ===== //
        [NotMapped]
        public string ProgressDisplay => GetProgressDisplay();

        private string GetProgressDisplay()
        {
            if (Product == null) return "";

            var unit = Product.MeasurementUnitDisplay;
            var quantityProgress = $"{TotalCommitted:N2} {unit} of {TargetQuantity:N2} {unit}";
            var amountProgress = $"₦{TotalPaid:N2} of ₦{TargetAmount:N2}";
            var amountPercent = ProgressPercentage;
            var quantityPercent = QuantityProgressPercentage;
            // var participantCount = $"{CurrentParticipants} members";
            
            return $"{quantityProgress} ({quantityPercent:F1}%) | {amountProgress} ({amountPercent:F1}%)";
        }

        [NotMapped]
        public string StatusDisplay => Status switch
        {
            TeamStatus.Active when IsExpired => "⏰ Expired",
            TeamStatus.Active when IsCritical => "🚨 Critical - 2h left",
            TeamStatus.Active when IsExpiringSoon => "🔥 Expiring Soon",
            TeamStatus.Active when IsSuccessful => "🎯 All Targets Reached",
            TeamStatus.Active when IsAmountTargetReached => "💰 Payment Target Reached",
            TeamStatus.Active when IsQuantityTargetReached => "📦 Quantity Target Reached",
            TeamStatus.Active => IsTargetReached ? "Completed" : "Active",
            TeamStatus.Completed => "Completed",
            TeamStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };

        [NotMapped]
        public string CountdownDisplay => GetCountdownDisplay();

        private string GetCountdownDisplay()
        {
            if (IsExpired) return "❌ Expired";
            
            var timeLeft = TimeRemaining;
            
            if (timeLeft.TotalMinutes <= 60)
                return $"🚨 {timeLeft.Minutes}m left";
            if (timeLeft.TotalHours <= 6)
                return $"🔥 {timeLeft.Hours}h {timeLeft.Minutes}m left";
            if (timeLeft.TotalHours <= 24)
                return $"⚡ {timeLeft.Hours} hours left";
            
            return $"⏰ {timeLeft.Days}d {timeLeft.Hours}h left";
        }

        [NotMapped]
        public string UrgencyLevel => GetUrgencyLevel();
        
        private string GetUrgencyLevel()
        {
            if (IsExpired) return "expired";
            if (IsCritical) return "critical";
            if (IsExpiringSoon) return "urgent";
            if (TimeRemaining.TotalHours <= 24) return "moderate";
            return "normal";
        }
    }
}