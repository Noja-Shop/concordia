using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Core.Models.TeamDTO;

namespace Noja.Application.Services.TeamManagement.Interface
{

    // <summary>
    // Main logic interface for Team management operations
    // This interface defines the methods that will be implemented by the TeamService class
    // </summary>
    public interface ITeamService
    {
        /// BUSINESS RULES:
        /// - Creator must specify their own quantity (can't create empty team)
        /// - Target quantity must be greater than creator's quantity
        /// - Product must be available and active
        /// - Creator quantity must be at least 1
        /// - Team expires in exactly 72 hours from creation
        /// 
        /// EXAMPLE: Customer wants 5kg rice, creates team targeting 50kg total

        //<summary>
        // Create a new team 
        // param customerId: ID of the customer creating the team
        // param createTeamDto: DTO containing team creation details
        // </summary>
        Task<ServiceResponse<TeamDto>> CreateTeamAsync(string customerId, CreateTeamDto createTeamDto);

        /// BUSINESS RULES:
        /// - Customer can only join each team once
        /// - Team must be active (not expired, cancelled, or completed)
        /// - Requested quantity + current committed ≤ target quantity
        /// - Payment must be processed successfully
        /// - If team reaches targets after join, mark as completed

        /// <summary>
        /// Join an existing team
        /// <param name="customerId">ID of the customer joining the team</param>
        /// <param name="joinTeamDto">DTO containing team joining details</param>
        /// </summary>
        Task<ServiceResponse<TeamMemberDto>> JoinTeamAsync(string customerId, JoinTeamDto joinTeamDto);

        /// <summary>
        /// Gets basic team information without member details.
        /// 
        /// USE CASE: Team listing pages, search results, quick previews
        /// PERFORMANCE: Fast - doesn't load member relationships
        /// 
        /// RETURNS:
        /// - Team basic info (name, description, targets, progress)
        /// - Current statistics (members count, total committed, progress %)
        /// - Status information (active, expired, completed)
        /// - Time remaining and urgency level
        /// <param name="teamId">ID of the team to retrieve</param>
        /// </summary>
        Task<ServiceResponse<TeamDto>> GetTeamDetailsAsync(Guid teamId);

        /// <summary>
        /// Gets complete team information including all member details.
        /// Show team details page, show who joined and their contributions.
        /// RETURNS:
        /// - Everything from GetTeamDetailsAsync() PLUS:
        /// - List of all team members with their quantities and payment status
        /// - Member roles (creator vs regular member)
        /// - Individual member payment statuses
        /// </summary>
        Task<ServiceResponse<TeamDto>> GetTeamDetailsWithMembersAsync(Guid teamId);
        
        /// <summary>
        /// Get all teams created by a specific customer has joined or created
        /// USE CASE: Customer's "My Teams" page, order history
        /// INCLUDES:
        /// - Teams created by this customer (as team leader)
        /// - Teams joined by this customer (as member)
        /// - All statuses (active, completed, expired, cancelled)
        /// 
        /// SORTING: Usually newest first or by status priority
        /// 
        /// EXAMPLE: "Your Teams: 2 active, 5 completed, 1 expired"
        /// </summary>
        /// <param name="customerId">ID of the customer to retrieve teams for</param>
        Task<ServiceResponse<List<TeamDto>>> GetTeamsByCustomerAsync(string customerId);

        /// <summary>
        /// Get all teams buying a specific product
        /// USE CASE: Product details page, product-specific team listings
        /// USE CASE: Product details page showing "3 teams buying this product"
        /// 
        /// BUSINESS VALUE:
        /// - Helps customers find teams for products they want
        /// - Shows product popularity
        /// - Enables customers to compare team options for same product
        /// 
        /// INCLUDES: All team statuses to show complete picture
        /// <param name="productId">ID of the product to retrieve teams for</param>
        Task<ServiceResponse<List<TeamDto>>> GetTeamByProductAsync(Guid productId);
    }
}