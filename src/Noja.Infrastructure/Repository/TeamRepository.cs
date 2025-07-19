using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Noja.Core.Entity;
using Noja.Core.Enums.Team;
using Noja.Core.Interfaces.Repository.Teams;
using Noja.Infrastructure.Data;

namespace Noja.Infrastructure.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly NojaDbContext _context;

        public TeamRepository(NojaDbContext context)
        {
            _context = context;
        }

        public async Task<Team> CreateAsync(Team team)
        {
            var product = await _context.Products.FindAsync(team.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found", nameof(team.ProductId));
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException("Cannot create a team for an inactive product.");
            }

            var creatorExists = await _context.Users.AnyAsync(u => u.Id == team.CreatedBy);
            if (!creatorExists)
                throw new ArgumentException("Creator does not exist", nameof(team.CreatedBy));

            team.CreatedAt = DateTime.UtcNow;
            team.Status = TeamStatus.Active;

            // expiry
            team.InitializeExpiry();
            team.UnitPrice = product.UnitPrice;

            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
            return team;
        }

        public async Task<IEnumerable<Team>> GetActiveTeamsAsync()
        {
            var currentTime = DateTime.UtcNow;

            var teams = await _context.Teams
            .Include(p => p.Product)
            .Include(c => c.Creator)
            .Include(m => m.Members)
            .ThenInclude(p => p.Payment)
            .Where(t => t.Status == TeamStatus.Active && t.ExpiresAt > currentTime
            && t.Members.Sum(m => m.Payment.Status == PaymentStatus.Completed ? m.AmountPaid : 0) < t.TargetAmount)
            .OrderBy(t => t.ExpiresAt)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

            foreach(var team in teams)
            {
               team.CheckAndUpdateStatus();
            }
            return teams.Where(t => t.Status == TeamStatus.Active && t.CanJoin).ToList();

        }

        public async Task<Team> GetByIdAsync(Guid id)
        {
            var team = await _context.Teams
            .Include(p => p.Product)
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(t => t.Id == id);

            if (team != null)
            {
                team.CheckAndUpdateStatus();

                // Save any changes if any occurred
                if (_context.Entry(team).Property(t => t.Status).IsModified)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return team;
        }

        public async Task<Team> GetByIdWithMemberAsync(Guid id)
        {
            var team = await _context.Teams
            .Include(p => p.Product)
            .Include(c => c.Creator)
            .Include(m => m.Members).ThenInclude(p => p.Payment)
            .ThenInclude(c => c.Customer)
            .FirstOrDefaultAsync(t => t.Id == id);

            if (team != null)
            {
                team.CheckAndUpdateStatus();

                if (_context.Entry(team).Property(t => t.Status).IsModified)
                {
                    await _context.SaveChangesAsync();
                }

            }
            return team;
        }

        public async Task<IEnumerable<Team>> GetExpiringTeamsAsync(int hoursFromNow)
        {
            var currentTime = DateTime.UtcNow;
            var expiringWindow = currentTime.AddHours(hoursFromNow);

            var expTeams = await _context.Teams
            .Include(p => p.Product)
            .Include(c => c.Creator)
            .Include(m => m.Members).ThenInclude(p => p.Customer)
            .Include(m => m.Members).ThenInclude(p => p.Payment)
            .Where(t => t.Status == TeamStatus.Active
            && t.ExpiresAt > currentTime && t.ExpiresAt <= expiringWindow)
            .OrderBy(t => t.ExpiresAt)
            .ToListAsync();

            foreach(var team in expTeams)
            {
                team.CheckAndUpdateStatus();
            }

            await _context.SaveChangesAsync();
            return expTeams.Where(t => t.Status == TeamStatus.Active).ToList();

        }

        public async Task<IEnumerable<Team>> GetTeamsByCreatorAsync(string creatorId)
        {
            var teams = await _context.Teams
            .Include(t => t.Product)
            .Include(t => t.Members).ThenInclude(t => t.Customer)
            .Include(t => t.Members).ThenInclude(t => t.Payment)
            .Where(t => t.CreatedBy == creatorId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

            // Updates all team statuses
            foreach (var team in teams)
            {
                team.CheckAndUpdateStatus();
            }
            await _context.SaveChangesAsync();
            return teams;
        }

        public async Task<IEnumerable<Team>> GetTeamsByProductAsync(Guid productId)
        {
            var teams = await _context.Teams
            .Include(t => t.Product)
            .Include(t => t.Creator)
            .Include(t => t.Members).ThenInclude(t => t.Payment)
            .Where(t => t.ProductId == productId)
            .OrderBy(t => t.Status == TeamStatus.Active ? 0 : 1)
            .ThenBy(t => t.ExpiresAt)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

            foreach(var team in teams)
            {
                team.CheckAndUpdateStatus();
            }
            await _context.SaveChangesAsync();
            return teams;

        }

        public async Task<IEnumerable<Team>> GetTeamsByStatusAsync(TeamStatus status)
        {
            var teams = await _context.Teams
            .Include(t => t.Product)
            .Include(t => t.Creator)
            .Include(t => t.Members).ThenInclude(t => t.Customer)
            .Include(t => t.Members).ThenInclude(t => t.Payment)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

            var statusChanged = false;
            foreach (var team in teams)
            {
                var originalStatus = team.Status;
                team.CheckAndUpdateStatus();

                // track if any status has changed 
                if (team.Status != originalStatus)
                {
                    statusChanged = true;
                }
            }
            
            // only save if something changed
            if (statusChanged)
            {
                await _context.SaveChangesAsync();
            }

            if (status == TeamStatus.Active)
            {
                // returns only teams that are actually active
                return teams.Where(t => t.Status == TeamStatus.Active && t.CanJoin).ToList();
            }
            else
            {
                // returns the statues that match the requested status
                return teams.Where(t => t.Status == status).ToList();
            }
        }
    }
}