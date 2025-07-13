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

        public string CreatedBy { get; set; }
        public string CreatorName { get; set; }

        public TeamStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int CurrentParticipants { get; set; }
        public decimal TotalCommitted { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public bool IsTargetReached { get; set; }
        public bool CanJoin { get; set; }
        public string ProgressDisplay { get; set; }
        public string StatusDisplay { get; set; }

        public List<TeamMemberDto> Members {get; set;} = new List<TeamMemberDto>(); 
        
    }
}