using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Entity;
using Noja.Core.Enums.Team;

namespace Noja.Core.Interfaces.Repository.Teams
{

    // <summary>
    // Repository interface for Team data access operations
    // </summary>

    public interface ITeamRepository
    {
        // ====== Read Ops ====== //


        // <summary>
        // Get team basic info
        // </summary>
        Task<Team> GetByIdAsync(Guid id);


        // <summary
        // Get team with all members(for team details page)
        // </summary>
        Task<Team> GetByIdWithMemberAsync(Guid id);

        // ======= Write Ops ====== //

        // <summary>
        // Create new team (any customer can do this)
        // </summary>
        Task<Team> CreateAsync(Team team);

        // ===== Business specific Queries ===== //

        // <summary>
        // Get teams that are still accepting members
        // </summary>
        Task<IEnumerable<Team>> GetActiveTeamsAsync();

        // <summary>
        // Get teams created by a specific user 
        // </summary>
        Task<IEnumerable<Team>> GetTeamsByCreatorAsync(string creatorId);

        // <summary>
        // Get teams for specific product
        // </summary>
        Task<IEnumerable<Team>> GetTeamsByProductAsync(Guid productId);

        // <summary>
        // Get teams by Status (For Admin)
        // </summary>
        Task<IEnumerable<Team>> GetTeamsByStatusAsync(TeamStatus status);

        // ===== EXPIRY-RELATED QUERIES ===== //
        
        // <summary>
        // Get teams expiring within X hours (for notifications)
        // </summary>
        Task<IEnumerable<Team>> GetExpiringTeamsAsync(int hoursFromNow);
        
    }
}