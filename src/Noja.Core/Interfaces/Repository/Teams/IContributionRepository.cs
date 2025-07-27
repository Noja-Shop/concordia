using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Entity;

namespace Noja.Core.Interfaces.Repository.Teams
{
    public interface IContributionRepository
    {
        // <summary>
        // Create a new contribution
        // </summary>
        // <param name="contribution">The contribution to create</param>
        Task<Contribution> CreateContributionAsync(Contribution contribution);

        // <summary>
        // Get a contribution by its ID specific
        // </summary>
        // <param name="id">The ID of the contribution</param>
        Task<Contribution?> GetUserContributionAsync(Guid teamId, string customerId);

        // <summary>
        // Get a list of all contributions by a specific team
        // </summary>
        // <param name="teamId">The ID of the team</param>
        Task<IEnumerable<Contribution>> GetContributionsByTeamIdAsync(Guid teamId);

        // <summary>
        // Get quantities contributed to  team
        // </summary>
        // <param name="teamId">The ID of the team</param>
        Task<decimal> GetTotalQuantityByTeamIdAsync(Guid teamId);
    }
}