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
    public class TeamMemberRepository : ITeamMemberRepository
    {
        private readonly NojaDbContext _context;
        
        public TeamMemberRepository(NojaDbContext context)
        {
            _context = context;
        }

        public async Task<TeamMember> CreateAsync(TeamMember member)
        {
            var team = await _context.Teams
            .Include(t => t.Product)
            .Include(t => t.Members).ThenInclude(t => t.Payment)
            .FirstOrDefaultAsync(t => t.Id == member.TeamId);

            if (team == null)
                throw new ArgumentException("Team not found", nameof(member.TeamId));

            // check if team can accept the member
            var canJoin = team.CanMemberJoin(member.Quantity, out string reason);
            if (!canJoin)
                throw new InvalidOperationException($"Cannot join team: {reason}");

            // check if member already exists
            var existingMember = await _context.Users.AnyAsync(u => u.Id == member.CustomerId);
            if (!existingMember)
                throw new ArgumentException("Customer not found", nameof(member.CustomerId));

            // check if payment exists and belongs to customer
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == member.PaymentId && p.CustomerId == member.CustomerId && p.TeamId == member.TeamId);
            if (payment == null)
                throw new ArgumentException("Payment not found or does not belong to customer");

            // check if member already exists in team (prevents duplication)
            var existingTeamMember = await _context.Members
            .FirstOrDefaultAsync(tm => tm.TeamId == member.TeamId && tm.CustomerId == member.CustomerId);
            if (existingTeamMember != null)
                throw new InvalidOperationException("Member already exists in this team");

            // verify amount matches calculations

            var expectedAmount = member.Quantity * team.UnitPrice;

            if (Math.Abs(member.AmountPaid - expectedAmount) > 0.01m)
                throw new InvalidOperationException($"Amount paid {member.AmountPaid} does not match expected amount of ₦{expectedAmount:N2}");
            
            // set system value
            member.JoinedAt = DateTime.UtcNow;

            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();

            // update team status after adding member
            team.CheckAndUpdateStatus();
            await _context.SaveChangesAsync();
            return await GetByIdAsync(member.Id);
        }

        public async Task<IEnumerable<TeamMember>> GetByCustomerAsync(string customerId)
        {
            var member = await _context.Members
            .Include(t => t.Team).ThenInclude(t => t.Product)
            .Include(t => t.Payment)
            .Where(t => t.CustomerId == customerId)
            .OrderByDescending(t => t.JoinedAt)
            .ToListAsync();

            // update team statuses for the current info
            var teamUpdate = member.Select(m => m.Team).Distinct().ToList();
            foreach(var team in teamUpdate)
            {
                team.CheckAndUpdateStatus();
            }

            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<TeamMember> GetByIdAsync(Guid id)
        {
            var member = await _context.Members
            .Include(t => t.Customer)
            .Include(t => t.Team).ThenInclude(t => t.Product)
            .Include(t => t.Payment)
            .FirstOrDefaultAsync(t => t.Id == id);

            return member;
        }

        public async Task<TeamMember> GetByTeamAndCustomerAsync(Guid teamId, string customerId)
        {
            var member = await _context.Members
                .Include(m => m.Team).ThenInclude(t => t.Product)
                .Include(m => m.Customer)
                .Include(m => m.Payment)
                .FirstOrDefaultAsync(m => m.TeamId == teamId && m.CustomerId == customerId);

            return member;
        }

        public async Task<IEnumerable<TeamMember>> GetByTeamAsync(Guid teamId)
        {
            var member = await _context.Members
            .Include(t => t.Customer)
            .Include(t => t.Team)
            .Include(t => t.Payment)
            .Where(m => m.TeamId == teamId)
            .OrderBy(t => t.IsCreator ? 0 : 1)
            .ThenBy(t => t.JoinedAt)
            .ToListAsync();

            return member;

        }

        public async Task<int> GetTeamMemberCountAsync(Guid teamId)
        {
            var memberCount = await _context.Members.CountAsync(t => t.TeamId == teamId);
            return memberCount;   
        }

        public async Task<decimal> GetTeamTotalCommittedAsync(Guid teamId)
        {
            var result = await _context.Members
            .Where(t => t.Id == teamId)
            .GroupBy(t => t.TeamId)
            .Select(t => new {TotalQuantity = t.Sum(t => t.Quantity)})
            .FirstOrDefaultAsync();
            return result?.TotalQuantity ?? 0;
        }

        public async Task<decimal> GetTeamTotalPaidAsync(Guid teamId)
        {
            var paymentSummary = await _context.Members
            .Where(t => t.TeamId == teamId)
            .Join(_context.Payments, t => t.PaymentId, p => p.Id, (t, p) => new { t.AmountPaid, p.Status })
            .GroupBy(t => t.Status)
            .Select(t => new { Status = t.Key, Total = t.Sum(t => t.AmountPaid), Count = t.Count() })
            .ToListAsync();

            // only completed payments count
            var completedPayments = paymentSummary.FirstOrDefault(p => p.Status == PaymentStatus.Completed);

            return completedPayments?.Total ?? 0;

        }
    }
}