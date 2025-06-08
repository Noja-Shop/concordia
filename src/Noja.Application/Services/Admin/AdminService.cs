using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Noja.Application.Models.AdminDTO;
using Noja.Application.Models.Common;
using Noja.Application.Services.Admin.Interface;
using Noja.Core.Entity;
using Noja.Core.Interfaces.Repository;
using Noja.Infrastructure.Authentication;

namespace Noja.Application.Services.Admin
{
    public class AdminService : IAdminService
    {

        private readonly IAdminRepository _adminRepository;
        

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<ServiceResponse<AdminResponseDto>> LoginAsync(LoginDto logindto)
        {
           return await _adminRepository.LoginAsync(logindto);
        }

        public async Task<ServiceResponse<AdminResponseDto>> SignupAsync(AdminSignupDto signupdto)
        {
            return await _adminRepository.SignupAsync(signupdto);
        }
    }
}