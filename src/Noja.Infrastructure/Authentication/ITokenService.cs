using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noja.Core.Entity;


namespace Noja.Infrastructure.Authentication
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(NojaUser user);
    }
}