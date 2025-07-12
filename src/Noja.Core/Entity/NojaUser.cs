using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Noja.Core.Enums.Team;

namespace Noja.Core.Entity
{
    public abstract class NojaUser : IdentityUser
    {

        // Add a protected constructor
        protected NojaUser() { } 
               
        public UserType UserType {get; set;}

        //User properties
        public required string FirstName {get; set;}
        public required string LastName {get; set;}
        
        [NotMapped]
        public string Password {get; set;}

        [NotMapped]
        public string ConfirmPassword {get; set;}

    }

    public class Customer : NojaUser
    {
        public string State { get; set; }
        public string City { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public bool IsProfileComplete { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ===== Team and Payment Navigation ==== //
        public ICollection<Team> CreatedTeams { get; set; } = new List<Team>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();

         // ===== Helper Properties ===== //
        
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Count of teams this customer created
        /// </summary>
        [NotMapped]
        public int TeamsCreatedCount => CreatedTeams?.Count ?? 0;

        /// <summary>
        /// Count of teams this customer joined (including created)
        /// </summary>
        [NotMapped]
        public int TeamsJoinedCount => TeamMemberships?.Count ?? 0;

        /// <summary>
        /// Total amount paid across all teams
        /// </summary>
        [NotMapped]
        public decimal TotalAmountPaid => Payments?
            .Where(p => p.Status == PaymentStatus.Completed)
            .Sum(p => p.Amount) ?? 0;

        /// <summary>
        /// Active teams this customer is participating in
        /// </summary>
        [NotMapped]
        public IEnumerable<Team> ActiveTeams => TeamMemberships?
            .Where(tm => tm.Team.Status == TeamStatus.Active)
            .Select(tm => tm.Team) ?? Enumerable.Empty<Team>();

        /// <summary>
        /// Completed teams this customer participated in
        /// </summary>
        [NotMapped]
        public IEnumerable<Team> CompletedTeams => TeamMemberships?
            .Where(tm => tm.Team.Status == TeamStatus.Completed)
            .Select(tm => tm.Team) ?? Enumerable.Empty<Team>();

        /// <summary>
        /// Check if customer can create new teams (based on profile completion)
        /// </summary>
        [NotMapped]
        public bool CanCreateTeams => IsProfileComplete;

        // ===== Business Logic Methods ===== //

        /// <summary>
        /// Check if customer is already in a specific team
        /// </summary>
        public bool IsInTeam(Guid teamId)
        {
            return TeamMemberships?.Any(tm => tm.TeamId == teamId) ?? false;
        }

        /// <summary>
        /// Get customer's membership in a specific team
        /// </summary>
        public TeamMember GetTeamMembership(Guid teamId)
        {
            return TeamMemberships?.FirstOrDefault(tm => tm.TeamId == teamId);
        }

        /// <summary>
        /// Check if customer created a specific team
        /// </summary>
        public bool IsTeamCreator(Guid teamId)
        {
            return CreatedTeams?.Any(t => t.Id == teamId) ?? false;
        }
    }

    public class Seller : NojaUser
    {
        
    }

    public enum UserType
    {
        Customer,
        Seller
    }
}