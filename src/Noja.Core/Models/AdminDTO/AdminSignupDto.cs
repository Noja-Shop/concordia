using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Application.Models.AdminDTO
{
    public class AdminSignupDto
    {
        [Required]
        public string FirstName {get; set;}

        [Required]
        public string LastName {get; set;}
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password {get; set;}

        [Required]
        [Compare("Password")]
        public string ConfirmPassword {get; set;}
    }
}