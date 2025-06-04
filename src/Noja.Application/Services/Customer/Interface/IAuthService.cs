using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Application.Models.Common;
using Noja.Application.Models.UserDTO;

namespace Noja.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<ServiceResponse<UserResponseDto>> SignupAsync(CustomerSignupDto signUpdto);
        Task<ServiceResponse<UserResponseDto>> LoginAsync(LoginDto logindto);
    }

}