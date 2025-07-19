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
    public class PaymentRepository : IPaymentRepository
    {

        private readonly NojaDbContext _context;
        public PaymentRepository(NojaDbContext context)
        {
            _context = context;
        }
        public async Task<Payment> CreateAsync(Payment payment)
        {
            try
            {
                if (payment == null)
                {
                    throw new ArgumentNullException(nameof(payment), "Payment cannot be null");
                }
                if (payment.Amount <= 0)
                {
                    throw new ArgumentException("Payment amount must be greater than zero", nameof(payment.Amount));
                }


                payment.CreatedAt = DateTime.UtcNow;
                if (payment.Status == PaymentStatus.Pending)
                {
                    payment.Status = PaymentStatus.Processing;
                }
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();
                return payment;

            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while creating the payment.", ex);
            }
           
        }

        public async Task<IEnumerable<Payment>> GetByCustomerAsync(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                    return new List<Payment>();

                var payments = await _context.Payments
                .Include(p => p.Team)
                .ThenInclude(t => t.Product)
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

                return payments;
        
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving payments by customer.", ex);
            }
        }

        public async Task<Payment> GetByIdAsync(Guid Id)
        {
            try
            {
                if (Id == Guid.Empty)
                    return null;

                var payment = await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == Id);
                return payment;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving the payment by ID.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            try
            {
                var payments = await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.Team)
                .ThenInclude(t => t.Product)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
                return payments;
                
            }

            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving payments by status.", ex);
            }
        }

        public async Task<Payment> GetByTeamAndCustomer(Guid teamId, string customerId)
        {
            try
            {
                if (teamId == Guid.Empty || string.IsNullOrEmpty(customerId))
                    return null;
                    
                var payment = await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.TeamId == teamId && p.CustomerId == customerId);
                return payment;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving the payment by team and customer.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetByTeamAsync(Guid teamId)
        {
            try
            {
                if (teamId == Guid.Empty)
                    return new List<Payment>();

                var payments = await _context.Payments
                .Include(p => p.Customer)
                .Where(p => p.TeamId == teamId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

                return payments;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving payments by team.", ex);
            }
        }
    }
}