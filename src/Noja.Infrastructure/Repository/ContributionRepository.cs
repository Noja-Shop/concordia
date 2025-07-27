using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Noja.Core.Entity;
using Noja.Core.Interfaces.Repository.Teams;
using Noja.Infrastructure.Data;

namespace Noja.Infrastructure.Repository
{
    public class ContributionRepository : IContributionRepository
    {

        private readonly NojaDbContext _context;

        public ContributionRepository(NojaDbContext context)
        {
            _context = context;
        }
        
        public async Task<Contribution> CreateContributionAsync(Contribution contribution)
        {
            await _context.Contributions.AddAsync(contribution);
            await _context.SaveChangesAsync();
            return contribution;
        }

        public async Task<IEnumerable<Contribution>> GetContributionsByTeamIdAsync(Guid teamId)
        {
            return await _context.Contributions
                .Where(c => c.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<Contribution?> GetUserContributionAsync(Guid teamId, string customerId)
        {
            return await _context.Contributions
                .FirstOrDefaultAsync(c => c.TeamId == teamId && c.CustomerId == customerId);
        }

        public async Task<decimal> GetTotalQuantityByTeamIdAsync(Guid teamId)
        {
           return await _context.Contributions
                .Where(c => c.TeamId == teamId)
                .SumAsync(c => c.Quantity);
        }
    }
}