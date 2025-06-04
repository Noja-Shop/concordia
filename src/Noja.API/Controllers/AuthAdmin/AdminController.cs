using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Noja.Application.Models.AdminDTO;
using Noja.Application.Services.Admin;
using Noja.Application.Services.Admin.Interface;

namespace Noja.API.Controllers.AuthAdmin
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost(Endpoints.AuthEndpoints.AuthAPIEndpoints.Seller.AdminSignup)]
        public async Task<IActionResult> SignupAdmin([FromBody] AdminSignupDto signupdto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminService.SignupAsync(signupdto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);

        }


        [HttpPost(Endpoints.AuthEndpoints.AuthAPIEndpoints.Seller.AdminLogin)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto logindto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminService.LoginAsync(logindto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

    }
}