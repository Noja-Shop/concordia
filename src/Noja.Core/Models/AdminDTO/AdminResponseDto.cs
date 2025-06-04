using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Application.Models.AdminDTO
{
    public class AdminResponseDto
    {
        public bool Success {get; set;}
        public string Token {get; set;}
        public string UserId {get; set;}
        public string RefreshToken{get; set;}
        public string Message {get; set;}
    }
}