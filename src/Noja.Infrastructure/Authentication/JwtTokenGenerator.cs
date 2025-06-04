using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Noja.Core.Entity;
using Noja.Infrastructure.Options;

namespace Noja.Infrastructure.Authentication
{
    public class JwtTokenGenerator : ITokenService
    {
        private readonly  JwtOption _jwtOption;
        private readonly IConfiguration _configuration;
        public JwtTokenGenerator(IOptions<JwtOption> jwtOptions, IConfiguration configuration)
        {
            _jwtOption = jwtOptions.Value;
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtToken(NojaUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserType",user.UserType.ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer : _configuration["Jwt:ValidIssuer"],
                audience : _configuration["Jwt:ValidAudience"],
                claims : claims,
                expires : DateTime.Now.AddDays(4),
                signingCredentials : creds

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}