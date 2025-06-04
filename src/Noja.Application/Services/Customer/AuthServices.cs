using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Noja.Application.Models.Common;
using Noja.Application.Models.UserDTO;
using Noja.Core.Entity;
using Noja.Infrastructure.Authentication;


namespace Noja.Application.Services.Auth
{
    public class AuthServices : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<NojaUser> _userManager;

        public AuthServices(ITokenService tokenService, UserManager<NojaUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }


        public async Task<ServiceResponse<UserResponseDto>> LoginAsync(LoginDto logindto)
        {
            var response = new ServiceResponse<UserResponseDto>();

            try
            {
                var user = await _userManager.FindByEmailAsync(logindto.Email);
                if(user == null || !await _userManager.CheckPasswordAsync(user, logindto.Password))
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }
                var token = await _tokenService.GenerateJwtToken(user);

                response.Success = true;
                response.Data = new UserResponseDto
                {
                    Success = true,
                    Token = token,
                    Message = "User logged in successfully",
                    UserId = user.Id

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

        public async Task<ServiceResponse<UserResponseDto>> SignupAsync(CustomerSignupDto signUpdto)
        {
            var response = new ServiceResponse<UserResponseDto>();

            try
            {
                var user = await _userManager.FindByEmailAsync(signUpdto.Email);

                if (user != null)
                {
                   response.Success = false;
                   response.Message = "User with this email exists";   
                }

                var newUser = new Customer
                {
                    FirstName = signUpdto.FirstName,
                    LastName = signUpdto.LastName,
                    UserName = signUpdto.Email,
                    Email = signUpdto.Email,
                    Password = signUpdto.Password,
                    UserType = UserType.Seller
                };

                var result = await _userManager.CreateAsync(newUser, signUpdto.Password);

                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = "Failed to create user";
                }

                var token = await _tokenService.GenerateJwtToken(newUser);

                response.Data = new UserResponseDto
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
                response.Message = $"An error occured {ex.Message}";
                return response;
            }
        }


    }
}