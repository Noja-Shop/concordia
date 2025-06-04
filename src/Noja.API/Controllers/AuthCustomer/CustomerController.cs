using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Noja.Application.Models.UserDTO;
using Noja.Application.Services.Auth;

namespace Noja.API.Controllers.Auth
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogoutService _logoutService;
        private readonly IProfileService _profileService;

        public CustomerController(IAuthService authService, ILogoutService logoutService, 
        IProfileService profileService)
        {
            _authService = authService;
            _logoutService = logoutService;
            _profileService = profileService;
        }

        [HttpPost(Endpoints.AuthEndpoints.AuthAPIEndpoints.Customer.CustomerSignup)]
        public async Task<IActionResult> SignupCustomer([FromBody] CustomerSignupDto signUpdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SignupAsync(signUpdto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost(Endpoints.AuthEndpoints.AuthAPIEndpoints.Customer.CustomerLogin)]
        public async Task<IActionResult> Login([FromBody] LoginDto logindto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(logindto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }

        [Authorize]
        [HttpPost(Endpoints.AuthEndpoints.AuthAPIEndpoints.Customer.CustomerLogout)]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new {Success = false, Message = "No token provided"});
            }

            var result = await _logoutService.LogoutAsync(token);
            if (!result)
            {
                return BadRequest(new {Success = false, Message = "Logout failed"});
            }

            return Ok(new {Success = true, Message = "Logout successful"});
        }

        [Authorize]
        [HttpPut(Endpoints.AuthEndpoints.AuthAPIEndpoints.Customer.CustomerUpdateProfile)]
        public async Task<IActionResult> UpdateProfile([FromBody] CustomerProfileUpdateDto profileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _profileService.UpdateSellerProfileAsync(userId, profileDto);

            if(!result.Success)
            return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet(Endpoints.AuthEndpoints.AuthAPIEndpoints.Customer.CustomerProfile)]
        public async Task<IActionResult> GetProfile()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _profileService.GetCustomerProfileAsync(userId);

            if(!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("/test")]
        public IActionResult Get()
        {
            return Ok("Authorized Seller: Create a shop");
        }
    }
}