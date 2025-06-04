using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Application.Models.AdminDTO
{
    public class LoginDto
    {
        public required string Email {get; set;}
        public required string Password {get; set;}
    }
}