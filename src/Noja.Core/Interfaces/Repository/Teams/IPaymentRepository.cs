using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Entity;
using Noja.Core.Enums.Team;

namespace Noja.Core.Interfaces.Repository.Teams
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(Guid Id);

        // <summary>
        // Make a payment
        // </summary>
        Task<Payment> CreateAsync(Payment payment);

        // ===== Business queries ===== //

        // <summary>
        // Get payment info of a specific customer
        // </summary>
        Task<IEnumerable<Payment>> GetByCustomerAsync(string customerId);

        // <summary>
        // Get the payment info of a specific Team
        // </summary>
        Task<IEnumerable<Payment>> GetByTeamAsync(Guid teamId);

        // <summary>
        // Get the payment info of both a specific customer and Team
        // </summary>
        Task<Payment> GetByTeamAndCustomer(Guid teamId, string customerId);

        // ==== Status queries ====== //

        // <summary>
        // Get all payments based on status
        // </summary>
        Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);
        
          
    }
}