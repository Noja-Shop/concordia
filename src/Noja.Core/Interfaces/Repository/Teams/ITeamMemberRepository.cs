using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Entity;


namespace Noja.Core.Interfaces.Repository.Teams
{
    // <summary>
    // Repository interface for Team data access operations
    // </summary>
    public interface ITeamMemberRepository
    {

        // <summary>
        // Get Team member basic info 
        // </summary>
        Task<TeamMember> GetByIdAsync(Guid id);

        // <summary>
        // Create a team and a member
        // </summary>
        Task<TeamMember> CreateAsync(TeamMember member);

        // <summary>
        // Get team and the customer in a team
        // </summary>
        Task<TeamMember> GetByTeamAndCustomerAsync(Guid teamId, string customerId);

        // <summary>
        // Get all members in teams
        // </summary>
        Task<IEnumerable<TeamMember>> GetByTeamAsync(Guid teamId);

        // <summary>
        // Get a customer in a team specific
        // </summary>

        Task<IEnumerable<TeamMember>> GetByCustomerAsync(string customerId);

        // === Validation queries === //
        Task<int> GetTeamMemberCountAsync(Guid teamId);

        // <summary>
        // Get the total committed by members of a specific team
        // </summary>
        Task<decimal> GetTeamTotalCommittedAsync(Guid teamId);

        // <summary>
        // Get the total payment of a specific team
        // </summary>
        Task<decimal> GetTeamTotalPaidAsync(Guid teamId);


    }
}