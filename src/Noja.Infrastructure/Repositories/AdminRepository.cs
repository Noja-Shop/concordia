using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Noja.Application.Models.AdminDTO;
using Noja.Application.Models.Common;
using Noja.Core.Entity;
using Noja.Core.Interfaces.Repository;
using Noja.Infrastructure.Authentication;

namespace Noja.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {

        private readonly ITokenService _tokenService;
        private readonly UserManager<NojaUser> _userManager;

        public AdminRepository(ITokenService tokenSevice, UserManager<NojaUser> userManager)
        {
            _tokenService = tokenSevice;
            _userManager = userManager;
        }
        public async Task<ServiceResponse<AdminResponseDto>> LoginAsync(LoginDto logindto)
        {
            var response = new ServiceResponse<AdminResponseDto>();

            try
            {
                var user = await _userManager.FindByEmailAsync(logindto.Email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, logindto.Password))
                {
                    response.Success = false;
                    response.Message = "User already exist";
                    return response;
                }

                var token = await _tokenService.GenerateJwtToken(user);

                response.Data = new AdminResponseDto
                {
                    Success = true,
                    Token = token,
                    Message = "User logged in successfully",
                    UserId = user.Id,
                };

                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occured {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<AdminResponseDto>> SignupAsync(AdminSignupDto signupdto)
        {
            var response = new ServiceResponse<AdminResponseDto>();

            try
            {
                var user = await _userManager.FindByEmailAsync(signupdto.Email);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User already exists";
                }

                var newUser = new Seller
                {
                    FirstName = signupdto.FirstName,
                    LastName = signupdto.LastName,
                    UserName = signupdto.Email,
                    Email = signupdto.Email,
                    Password = signupdto.Password,
                    UserType = UserType.Seller
                };

                var result = await _userManager.CreateAsync(newUser, signupdto.Password);

                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = "Failed to create user";
                }

                var token = await _tokenService.GenerateJwtToken(newUser);
                response.Data = new AdminResponseDto
                {
                    Success = true,
                    Token = token,
                    Message = "User created successfully",
                    UserId = newUser.Id
                };

                return response;
            }

            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occured{ex.Message}";
                return response;
            }
        }
    }
}