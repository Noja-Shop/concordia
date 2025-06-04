using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Noja.Application.Models.Common;
using Noja.Application.Models.UserDTO;
using Noja.Core.Entity;

namespace Noja.Application.Services.Auth
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<NojaUser> _userManager;

        public ProfileService(UserManager<NojaUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ServiceResponse<string>> UpdateSellerProfileAsync(string userId, CustomerProfileUpdateDto profileDto)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Seller not found";
                    return response;
                }

                if (user is not Customer customer)
                {
                    response.Success = false;
                    response.Message = "User is not a seller";
                    return response;
                }


                //seller update the remaining properties

                customer.SellerPhoneNumber = profileDto.CustomerPhoneNumber;
                customer.State = profileDto.State;
                customer.StreetAddress = profileDto.StreetAddress;
                customer.City = profileDto.City;
                customer.IsProfileComplete = true;
                customer.UpdatedAt = DateTime.UtcNow;

                var update = await _userManager.UpdateAsync(customer);
                if (!update.Succeeded)
                {
                    response.Success = false;
                    response.Message = "Failed to update seller profile";
                    response.Errors = update.Errors;
                    return response;
                }

                response.Success = true;
                response.Message = "Profile updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occured {ex.Message}";
                return response;
            }
        }

        public async Task<ServiceResponse<CustomerProfileDto>> GetCustomerProfileAsync(string userId)
        {
            var response = new ServiceResponse<CustomerProfileDto>();

            try
            {   
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Seller not found";
                    return response;
                }

                if (user is not Customer customer)
                {
                    response.Success = false;
                    response.Message = "User is not a seller";
                    return response;
                }

                var profileDto = new CustomerProfileDto
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    SellerPhoneNumber = customer.SellerPhoneNumber,
                    Email = customer.Email,
                    State = customer.State,
                    City = customer.City,
                    StreetAddress = customer.StreetAddress, 
                };

                response.Success = true;
                response.Data = profileDto;
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