using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.UserDTO;

namespace Noja.Application.Services.Auth
{
    public interface IProfileService
    {
        Task<ServiceResponse<string>> UpdateSellerProfileAsync
        (string userId, CustomerProfileUpdateDto profileDto);

        Task<ServiceResponse<CustomerProfileDto>> GetCustomerProfileAsync(string userId);
    }
}