using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Core.Entity;
using Noja.Core.Enums.Team;
using Noja.Core.Models.TeamDTO;

namespace Noja.Application.Services.TeamManagement.Interface
{

    /// <summary>
    /// Handles simulated payment processing 
    /// Every team participation requires payment - no free riders!
    /// - Team creator pays when creating team
    /// - Team members pay when joining team
    /// - Payment is simulated but mandatory for transaction completion
    /// 
    /// SIMULATION: Uses SimulateSuccess flag and delay to mimic real payment processing
    /// This proves the payment flow works before integrating real payment gateway
    /// </summary>
    public interface IPaymentService
    {
        Task<ServiceResponse<PaymentDto>> ProcessPaymentAsync(Payment payment);

        /// <summary>
        /// Gets payment details for customer or admin view
        /// USE CASE: 
        /// - Customer wants to see their payment status
        /// - Admin wants to check payment details
        /// - Team creator wants to see who has paid
        /// - Customer can see what they have paid for
        /// <param name="paymentId">Unique identifier of the payment</param>
        /// </summary>
        Task<ServiceResponse<PaymentDto>> GetPaymentDetailsAsync(Guid paymentId);

        /// <summary>
        /// Gets all payments by a specifi customer
        /// USE CASE: 
        /// - Customer's "My Payments" or "My Teams" 
        /// - Shows a customer their spending 
        /// - Which teams they paid for              
        /// </summary>
        /// <param name="customerId">ID of the customer</param>
        /// <returns>>ServiceResponse with List of PaymentDto for customer's payments</returns> 
        Task<ServiceResponse<List<PaymentDto>>> GetCustomerPaymentsAsync(string customerId);

        /// <summary>
        /// Gets all payments for specific team
        /// USE CASE:
        /// - Shows total money collected vs target
        /// - Team creator sees who has paid and who hasn't
        /// - Team details page showing payment status of all members
        /// <param name="teamId">ID of the team</param>
        /// </summary>
        Task<ServiceResponse<List<PaymentDto>>> GetTeamPaymentsAsync(Guid teamId);

        /// <summary>
        /// Creates a payment record for team participation. Ensures all team participants creates a payment
        /// USE CASE:
        /// - Calculates payment amount (quantity × unit price)
        /// - Creates Payment entity with simulation settings
        /// - Links payment to customer and team
        /// - Sets initial status to "Pending"
        /// <param name="customerId">ID of the customer making payment</param>
        /// <param name="teamId">ID of the team being paid for</param>
        /// <param name="amount">Payment amount calculated from quantity × unit price</param>
        /// <param name="paymentMethod">Payment method selected by customer</param>
        /// <param name="simulateSuccess">Whether to simulate success or failure</param>
        /// </summary>
        Task<ServiceResponse<Payment>> CreatePaymentAsync(string customerId, Guid teamId, decimal amount, PaymentMethod paymentMethod, bool simulateSuccess); 
    }
}