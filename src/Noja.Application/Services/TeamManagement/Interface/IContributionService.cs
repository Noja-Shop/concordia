using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Core.Entity;
using Noja.Core.Models.TeamDTO;

namespace Noja.Application.Services.TeamManagement.Interface
{
    public interface IContributionService
    {
        Task<ServiceResponse<Contribution>> AddContributionAsync(CreateContributionDto contributionDto, bool IsCreator = false);
    }
}