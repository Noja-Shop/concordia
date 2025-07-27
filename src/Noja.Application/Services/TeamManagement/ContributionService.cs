using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Services.TeamManagement.Interface;
using Noja.Core.Entity;
using Noja.Core.Interfaces.Repository.Teams;
using Noja.Core.Models.TeamDTO;

namespace Noja.Application.Services.TeamManagement
{
    public class ContributionService : IContributionService
    {
        private readonly IContributionRepository _contributionRepository;
        private readonly ITeamRepository _teamRepository;

        public ContributionService(IContributionRepository contributionRepository,ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
            _contributionRepository = contributionRepository;
        }
        
        public async Task<ServiceResponse<Contribution>> AddContributionAsync(CreateContributionDto contributionDto, bool IsCreator = false)
        { 
            var team = await _teamRepository.GetByIdAsync(contributionDto.TeamId);
            if (team == null)
            {
                return new ServiceResponse<Contribution>
                {
                    Success = false,
                    Message = "Team not found",
                };
            }

            if (!IsCreator)
            {
                var existingContribution = await _contributionRepository.GetUserContributionAsync(contributionDto.TeamId, contributionDto.CustomerId);
                if (existingContribution != null)
                {
                    return new ServiceResponse<Contribution>
                    {
                        Success = false,
                        Message = "Contribution already exists for this user in the team."
                    };
                }

                var totalQuantity = await _contributionRepository.GetTotalQuantityByTeamIdAsync(contributionDto.TeamId);
                if (totalQuantity + contributionDto.Quantity > team.TargetQuantity)
                {
                    return new ServiceResponse<Contribution>
                    {
                        Success = false,
                        Message = "Total contributions exceed the team's quantity limit."
                    };
                }
            }

            var contribution = new Contribution
            {
                TeamId = contributionDto.TeamId,
                CustomerId = contributionDto.CustomerId,
                Quantity = contributionDto.Quantity,
                PaymentId = contributionDto.PaymentId
            };

            var createdContribution = await _contributionRepository.CreateContributionAsync(contribution);
            return new ServiceResponse<Contribution> { Data = createdContribution };
        }
        
    }
}