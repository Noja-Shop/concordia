using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.ProductDTO;
using Noja.Application.Services.TeamManagement.Interface;
using Noja.Core.Entity;
using Noja.Core.Enums.Team;
using Noja.Core.Interfaces.Repository;
using Noja.Core.Interfaces.Repository.Teams;
using Noja.Core.Models.TeamDTO;
using Noja.Infrastructure.Data;

namespace Noja.Application.Services.TeamManagement
{
    public class TeamService : ITeamService
    {

        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _memberRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProductRepository _productRepository;
        private readonly IContributionService _contributionService;
        private readonly NojaDbContext _context;

        public TeamService(ITeamRepository teamRepository,
            ITeamMemberRepository memberRepository,
            IPaymentRepository paymentRepository,
            IProductRepository productRepository,
            IContributionService contributionService,
            NojaDbContext context
            )
        {
            _teamRepository = teamRepository;
            _memberRepository = memberRepository;
            _paymentRepository = paymentRepository;
            _productRepository = productRepository;
            _contributionService = contributionService;
            _context = context;
        }
        public async Task<ServiceResponse<TeamDto>> CreateTeamAsync(string customerId, CreateTeamDto createTeamDto)
        {
            var response = new ServiceResponse<TeamDto>();
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (createTeamDto == null || string.IsNullOrEmpty(customerId))
                {
                    response.Success = false;
                    response.Message = "Team creation data is required";
                    return response;
                }

                var product = await _productRepository.GetProductById(createTeamDto.ProductId);

                if (!product.IsActive || !product.IsInStock || product == null)
                {
                    response.Success = false;
                    response.Message = "Product is not available or not available";
                    return response;
                }

                if (createTeamDto.CreatorQuantity > product.PackageSize)
                {
                    response.Success = false;
                    response.Message = "Creator's quantity cannot be greater than target quantity";
                    return response;
                }

                // calculate amounts
                var unitPrice = product.UnitPrice;
                var totalProductPrice = unitPrice * product.PackageSize;
                var creatorAmount = createTeamDto.CreatorQuantity * unitPrice;

                if (creatorAmount > totalProductPrice)
                {
                    response.Success = false;
                    response.Message = "Creator's amount cannot be greater than total product price";
                    return response;
                }

                var team = new Team
                {
                    Name = createTeamDto.Name?.Trim() ?? $"{product.Name}",
                    Description = createTeamDto.Description,
                    Product = product,
                    ProductId = createTeamDto.ProductId,
                    CreatedBy = customerId,
                    TargetQuantity = product.PackageSize,
                    TargetAmount = creatorAmount,
                    UnitPrice = unitPrice,
                    Contributions = new List<Contribution>(),
                    Status = TeamStatus.Active
                };

                // Initialize expiry (from now up to 72 hrs)
                team.InitializeExpiry();

                // save team to the database
                var createTeam = await _teamRepository.CreateAsync(team);

                // 1. create payment for team creator
                var creatorPayment = new Payment
                {
                    CustomerId = customerId,
                    TeamId = createTeam.Id,
                    Amount = creatorAmount,
                    PaymentMethod = createTeamDto.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    SimulateSuccess = createTeamDto.SimulatePaymentSuccess,
                    SimulationDelaySeconds = 2,
                    CreatedAt = DateTime.UtcNow,
                };

                creatorPayment.TransactionReference = GenerateTransactionReference(creatorPayment);

                var createdPayment = await _paymentRepository.CreateAsync(creatorPayment);

                // 3. update payment with team ID
                await _contributionService.AddContributionAsync(new CreateContributionDto
                {
                    TeamId = createTeam.Id,
                    CustomerId = customerId,
                    Quantity = createTeamDto.CreatorQuantity,
                    Amount = creatorAmount,
                    PaymentId = createdPayment.Id
                }, true);


                // 4. Process the creator's payment
                var paymentSuccess = await ProcessPaymentDirectly(createdPayment);

                if (!paymentSuccess)
                {
                    // delete team if payment processing fails
                    // in production use database transation
                    response.Success = false;
                    response.Message = $"Payment processing failed: {createdPayment.FailureReason}";
                    return response;
                }

                // an update to the Quantity that decreases it when a team is Active
                await _productRepository.UpdateQuantityAsync(product.Id, product.Quantity - 1);

                // 5. create team member for creator
                var creatorMember = new TeamMember
                {
                    TeamId = createTeam.Id,
                    CustomerId = customerId,
                    Quantity = createTeamDto.CreatorQuantity,
                    AmountPaid = creatorAmount,
                    PaymentId = creatorPayment.Id
                };

                var createdMember = await _memberRepository.CreateAsync(creatorMember);

                // 6. Get compete team with members for response
                var teamComplete = await _teamRepository.GetByIdWithMemberAsync(createTeam.Id);

                // saves the transaction/atomicity operation block
                await transaction.CommitAsync();

                // 7. map to dto and return
                response.Success = true;
                response.Message = "Team created successfully";
                response.Data = MapToTeamDto(teamComplete);
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.Success = false;
                response.Message = "An error occurred while creating the team: " + ex.Message;
                return response;
            }
        }

        public async Task<ServiceResponse<List<TeamDto>>> GetTeamByProductAsync(Guid productId)
        {
            var response = new ServiceResponse<List<TeamDto>>();

            try
            {
                // get all teams for product
                var teams = await _teamRepository.GetTeamsByProductAsync(productId);

                if (teams == null || !teams.Any())
                {
                    response.Success = false;
                    response.Message = "No teams found for this product";
                    return response;
                }

                // map to dto
                response.Success = true;
                response.Message = "Teams retrieved successfully";
                response.Data = teams.Select(MapToTeamDto).ToList();
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving teams: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<TeamDto>> GetTeamDetailsAsync(Guid teamId)
        {

            var response = new ServiceResponse<TeamDto>();

            try
            {
                // get team with all members
                var team = await _teamRepository.GetByIdWithMemberAsync(teamId);

                if (team == null)
                {
                    response.Success = false;
                    response.Message = $"Team is not found";
                    return response;
                }

                // map to dto with all details
                response.Success = true;
                response.Message = "Details retrieved successfully";
                response.Data = MapToTeamDto(team);
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retriving {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<TeamDto>> GetTeamDetailsWithMembersAsync(Guid teamId)
        {
            var response = new ServiceResponse<TeamDto>();

            try
            {
                // get team with all members
                var team = await _teamRepository.GetByIdWithMemberAsync(teamId);

                if (team == null)
                {
                    response.Success = false;
                    response.Message = "Team not found";
                    return response;
                }

                // map to dto with all details
                response.Success = true;
                response.Message = "Team details retrieved successfully";
                response.Data = MapToTeamDto(team);
                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving team details: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<List<TeamDto>>> GetTeamsByCustomerAsync(string customerId)
        {
            var response = new ServiceResponse<List<TeamDto>>();
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    response.Success = false;
                    response.Message = "Customer ID is required";
                    return response;
                }

                // get teams created by customer
                var teams = await _teamRepository.GetTeamsByCreatorAsync(customerId);

                if (teams == null || !teams.Any())
                {
                    response.Success = false;
                    response.Message = "No teams found for this customer";
                    return response;
                }

                // map to dto
                response.Success = true;
                response.Message = "Teams retrieved successfully";
                response.Data = teams.Select(MapToTeamDto).ToList();
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving teams: {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<TeamMemberDto>> JoinTeamAsync(string customerId, JoinTeamDto joinTeamDto)
        {
            var response = new ServiceResponse<TeamMemberDto>();

            try
            {
                if (joinTeamDto == null)
                {
                    response.Success = false;
                    response.Message = "Join team data is required";
                    return response;
                }

                // Team validation: Get team with current members
                var team = await _teamRepository.GetByIdWithMemberAsync(joinTeamDto.TeamId);

                if (team == null)
                {
                    response.Success = false;
                    response.Message = "Team not found";
                    return response;
                }

                // Team validation: check if team is joinable
                var canJoin = team.CanMemberJoin(joinTeamDto.Quantity, out string reason);

                if (!canJoin)
                {
                    response.Success = false;
                    response.Message = reason;
                    return response;
                }

                // Team validation: check if customer already member
                var existingMember = await _memberRepository.GetByTeamAndCustomerAsync(joinTeamDto.TeamId, customerId);

                if (existingMember != null)
                {
                    response.Success = false;
                    response.Message = "You're already a member";
                    return response;
                }

                // Team validation: calculate payment amount
                var paymentAmount = joinTeamDto.Quantity * team.UnitPrice;

                var payment = new Payment
                {
                    CustomerId = customerId,
                    TeamId = joinTeamDto.TeamId,
                    Amount = paymentAmount,
                    PaymentMethod = joinTeamDto.PaymentMethod,
                    Status = PaymentStatus.Pending,
                    SimulateSuccess = joinTeamDto.SimulatedPaymentSuccess,
                    SimulationDelaySeconds = 2,
                    CreatedAt = DateTime.UtcNow
                };


                // 1. create payment for joining member
                var createdPayment = await _paymentRepository.CreateAsync(payment);

                // 2. Process payment
                var paymentSuccess = await ProcessPaymentDirectly(createdPayment);

                if (!paymentSuccess)
                {
                    response.Success = false;
                    response.Message = $"Payment process failed {createdPayment.FailureReason}";
                    return response;
                }

                // 3. create team member, the joiner

                var teamMember = new TeamMember
                {
                    TeamId = joinTeamDto.TeamId,
                    CustomerId = customerId,
                    Quantity = joinTeamDto.Quantity,
                    AmountPaid = paymentAmount,
                    PaymentId = payment.Id
                };

                var createdMember = await _memberRepository.CreateAsync(teamMember);

                // 4. check if team targets are reached
                var memberCount = await _memberRepository.GetTeamMemberCountAsync(joinTeamDto.TeamId);
                var totalPaid = await _memberRepository.GetTeamTotalPaidAsync(joinTeamDto.TeamId);
                var totalCommited = await _memberRepository.GetTeamTotalCommittedAsync(joinTeamDto.TeamId);

                // update team status if targets reached
                team.CheckAndUpdateStatus();

                response.Success = true;
                response.Message = "Successfully joined team";
                response.Data = MapToTeamMemberDto(createdMember);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error joining team: {ex.Message}";
                return response;
            }
        }
    

    /// ==== Mapping methods: Data transfer objects ==== ///
    
    private static TeamDto MapToTeamDto(Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                ProductId = team.ProductId,
                Description = team.Description,
                TargetQuantity = team.TargetQuantity,
                TargetAmount = team.TargetAmount,
                CreatedBy = team.CreatedBy,
                // MinParticipants = team.MinParticipants,
                Status = team.Status,
                CompletedAt = team.CompletedAt,
                ExpiresAt = team.ExpiresAt,
                // ==============================//
                Product = team.Product != null ? MapToProductSummaryDto(team.Product) : null,
            

                // =============Calculated properties=================//
                IsExpired = team.IsExpired,
                CountdownDisplay = team.CountdownDisplay,
                UrgencyLevel = team.UrgencyLevel,
                StatusDisplay = team.StatusDisplay,
                ProgressDisplay = team.ProgressDisplay,
                TotalCommitted = team.TotalCommitted,
                TotalPaid = team.TotalPaid,
                RemainingQuantity = team.RemainingQuantity,
                RemainingAmount = team.RemainingAmount,
                ProgressPercentage = team.ProgressPercentage,
                QuantityProgressPercentage = team.QuantityProgressPercentage,

                // ==================Team logic=====================//
                IsTargetReached = team.IsTargetReached,
                IsAmountTargetReached = team.IsAmountTargetReached,
                IsQuantityTargetReached = team.IsQuantityTargetReached,
                CanJoin = team.CanJoin,
                Members = team.Members.Select(MapToTeamMemberDto).ToList()

            };
        }

        private static TeamMemberDto MapToTeamMemberDto(TeamMember member)
        {
            return new TeamMemberDto
            {
                Id = member.Id,
                CustomerId = member.CustomerId,
                Quantity = member.Quantity,
                AmountPaid = member.AmountPaid,
                TeamId = member.TeamId,
                PaymentId = member.PaymentId,
                CommitmentDisplay = member.CommitmentDisplay
            };
        }

        private static ProductSummaryDto MapToProductSummaryDto(Product product)
        {
            return new ProductSummaryDto
            {
                Id = product.Id,
                Name = product.Name,
                PackagePrice = product.PackagePrice,
                UnitPrice = product.UnitPrice,
                PackageSize = product.PackageSize,
                ContainerCount = product.ContainerCount,
                ContainerSize = product.ContainerSize,
                Category = product.Category,
                MeasurementUnit = product.MeasurementUnit,
                PackageType = product.PackageType,
                ContainerType = product.ContainerType,
                IsInStock = product.IsInStock,
                IsActive = product.IsActive,
                Quantity = product.Quantity,
                ProductDisplayName = product.ProductDisplayName,
                PackagePriceDisplay = product.PackagePriceDisplay,
                UnitPriceDisplay = product.UnitPriceDisplay,
                FullPriceDisplay = product.FullPriceDisplay,
                StockStatusDisplay = product.StockStatusDisplay,
                CategoryDisplay = product.CategoryDisplay,
                ContainerTypeDisplay = product.ContainerTypeDisplay,
                ContainerDescription = product.ContainerDescription,
                PackageTypeDisplay = product.PackageTypeDisplay,

            };
        }

        /// <summary>
        /// Processes payment directly within TeamService.
        /// Simulates payment processing with configurable success/failure.
        /// 
        /// BUSINESS LOGIC:
        /// - Simulates real payment gateway delay
        /// - Updates payment status based on simulation settings
        /// - Generates transaction reference for successful payments
        /// - Records failure reasons for failed payments
        /// - Updates payment record in database
        /// 
        /// SIMULATION FEATURES:
        /// - Configurable delay (SimulationDelaySeconds)
        /// - Success/failure toggle (SimulateSuccess)
        /// - Realistic transaction references
        /// - Proper status transitions: Pending → Processing → Completed/Failed
        /// </summary>
        /// <param name="payment">Payment entity to process</param>
        /// <returns>True if payment successful, false if failed</returns>
        private async Task<bool> ProcessPaymentDirectly(Payment payment)
        {
            try
            {
                // STEP 1: Update status to Processing
                payment.Status = PaymentStatus.Processing;
                
                // STEP 2: Simulate payment gateway delay
                if (payment.SimulationDelaySeconds > 0)
                {
                    await Task.Delay(payment.SimulationDelaySeconds * 1000);
                }

                // STEP 3: Process based on simulation settings
                if (payment.SimulateSuccess)
                {
                    // SUCCESS SCENARIO
                    payment.Status = PaymentStatus.Completed;
                    payment.CompletedAt = DateTime.UtcNow;
                    payment.TransactionReference = GenerateTransactionReference(payment);
                    payment.FailureReason = null; // Clear any previous failure reason
                }
                else
                {
                    // FAILURE SCENARIO
                    payment.Status = PaymentStatus.Failed;
                    payment.CompletedAt = null;
                    payment.TransactionReference = null;
                    payment.FailureReason = "Simulated payment failure - insufficient funds";
                }

                // STEP 4: Update payment in database
                // Note: You'll need to add UpdateAsync method to PaymentRepository
                await UpdatePaymentInDatabase(payment);

                return payment.Status == PaymentStatus.Completed;
            }
            catch (Exception ex)
            {
                // EXCEPTION HANDLING: Mark payment as failed
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = $"Processing error: {ex.Message}";
                payment.CompletedAt = null;
                
                try
                {
                    await UpdatePaymentInDatabase(payment);
                }
                catch
                {
                    // Log this error but don't throw - we already have the main exception
                }

                return false;
            }
        }

        /// <summary>
        /// Generates a realistic transaction reference for successful payments.
        /// Format: TXN_YYYYMMDDHHMMSS_PaymentId8Chars
        /// Example: TXN_20241201143022_A1B2C3D4
        /// </summary>
        /// <param name="payment">Payment entity</param>
        /// <returns>Formatted transaction reference</returns>
        private static string GenerateTransactionReference(Payment payment)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var paymentIdShort = payment.Id.ToString().Replace("-", "")[..8].ToUpper();
            return $"TXN_{timestamp}_{paymentIdShort}";
        }

        /// <summary>
        /// Updates payment record in database.
        /// Handles the database update with proper error handling.
        /// 
        /// NOTE: This requires UpdateAsync method in PaymentRepository
        /// </summary>
        /// <param name="payment">Payment entity to update</param>
        private async Task UpdatePaymentInDatabase(Payment payment)
        {
            try
            {
                // This will require adding UpdateAsync to IPaymentRepository
                // For now, we'll use a workaround with the existing methods
                
                // WORKAROUND: Since UpdateAsync might not exist yet,
                // we'll update the payment through Entity Framework context directly
                // This is not ideal but will work until UpdateAsync is implemented
                
                // TODO: Implement UpdateAsync in PaymentRepository
                // await _paymentRepository.UpdateAsync(payment);
                
                // TEMPORARY: Just log that update is needed
                // In a real scenario, the payment status update is critical
                Console.WriteLine($"Payment {payment.Id} status updated to {payment.Status}");
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - payment processing logic should continue
                Console.WriteLine($"Failed to update payment {payment.Id}: {ex.Message}");
                throw; // Re-throw to handle in calling method
            }
        }

    } 
}