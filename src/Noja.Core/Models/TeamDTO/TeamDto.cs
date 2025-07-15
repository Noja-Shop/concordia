using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.ProductDTO;
using Noja.Core.Enums.Team;

namespace Noja.Core.Models.TeamDTO
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid ProductId { get; set; }
        public ProductSummaryDto Product { get; set; }
        public decimal TargetQuantity { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal UnitPrice { get; set; }
        
        // expiry 
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired {get; set;}
        public string CountdownDisplay {get; set;}
        public string UrgencyLevel { get; set; }

        // participants
        public string CreatedBy { get; set; }
        public string CreatorName { get; set; }

        // Participation rules
        public int MinParticipants {get; set;}
        public int CurrentParticipants { get; set; }
        public bool HasMinParticipants { get; set; }

        // status info
        public TeamStatus Status { get; set; }
        public string StatusDisplay { get; set; }
        public string ProgressDisplay { get; set; }
        public DateTime? CompletedAt { get; set; }

        // progress info
        public decimal TotalCommitted { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public decimal ProgressAmount { get; set; }
        public decimal QuantityProgressPercentage { get; set; }

        // success info
        public bool IsTargetReached { get; set; }
        public bool IsAmountTargetReached {get; set;}
        public bool IsQuantityTargetReached { get; set; }
        public bool CanJoin { get; set; }
        
        public List<TeamMemberDto> Members {get; set;} = new List<TeamMemberDto>(); 
        
    }
}