using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Noja.Application.Services.TeamManagement.Interface;
using Noja.Core.Models.TeamDTO;

namespace Noja.API.Controllers.TeamManager
{
    [ApiController]

    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [Authorize]
        [HttpPost(Endpoints.TeamEndpoints.TeamAPIEnpoints.TeamManagement.CreateTeam)]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto createTeam)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _teamService.CreateTeamAsync(customerId, createTeam);

            if (!result.Success) return BadRequest(new { message = result.Message, errors = result.Errors });

            return CreatedAtAction(nameof(GetTeamDetails), new { id = result.Data.Id }, result.Data);

        }

        [Authorize]
        [HttpGet(Endpoints.TeamEndpoints.TeamAPIEnpoints.TeamManagement.GetTeamDetails)]
        public async Task<IActionResult> GetTeamDetails([FromRoute] Guid id)
        {
            var result = await _teamService.GetTeamDetailsAsync(id);

            if (!result.Success) return NotFound(result.Message);

            return Ok(result.Data);
        }

        [Authorize]
        [HttpGet(Endpoints.TeamEndpoints.TeamAPIEnpoints.TeamManagement.GetTeamDetailsWithMember)]
        public async Task<IActionResult> GetTeamDetailsWithMember([FromRoute] Guid id)
        {
            var result = await _teamService.GetTeamDetailsWithMembersAsync(id);

            if (!result.Success) return NotFound(result.Message);

            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost(Endpoints.TeamEndpoints.TeamAPIEnpoints.TeamManagement.JoinTeam)]
        public async Task<IActionResult> JoinTeam([FromBody] JoinTeamDto joinTeamDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _teamService.JoinTeamAsync(customerId, joinTeamDto);

            if (!result.Success) return BadRequest(new { message = result.Message, errors = result.Errors });

            return Ok(new { message = result.Message, data = result.Data });
        }

        [Authorize]
        [HttpGet(Endpoints.TeamEndpoints.TeamAPIEnpoints.TeamManagement.GetTeamsByCustomer)]
        public async Task<IActionResult> GetTeamsByCustomer()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _teamService.GetTeamsByCustomerAsync(customerId);

            if (!result.Success) return NotFound(result.Message);

            return Ok(result.Data);
        }

        [Authorize]
        [HttpGet(Endpoints.TeamEndpoints.TeamAPIEnpoints.TeamManagement.GetTeamByProduct)]
        public async Task<IActionResult> GetTeamByProduct([FromQuery] Guid productId)
        {
            var result = await _teamService.GetTeamByProductAsync(productId);

            if (!result.Success) return NotFound(result.Message);

            return Ok(result.Data);
        }
    }
}