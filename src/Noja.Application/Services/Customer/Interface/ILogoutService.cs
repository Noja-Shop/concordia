using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Application.Services.Auth
{
    public interface ILogoutService
    {
        Task<bool> LogoutAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}