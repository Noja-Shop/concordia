using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.AdminDTO;
using Noja.Application.Models.Common;

namespace Noja.Application.Services.Admin.Interface
{
    public interface IAdminService
    {
        Task<ServiceResponse<AdminResponseDto>> SignupAsync(AdminSignupDto signupdto);
        Task<ServiceResponse<AdminResponseDto>> LoginAsync(LoginDto logindto);
    }
}