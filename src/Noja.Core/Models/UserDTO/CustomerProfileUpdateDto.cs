using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.Application.Models.UserDTO
{
    public class CustomerProfileUpdateDto
    {
        [Required]
        [Phone]
        public string CustomerPhoneNumber {get; set;}

        [Required]
        public string State {get; set;}

        [Required]
        public string City {get; set;}
        
        [Required]
        public string StreetAddress {get;set;}
    }
}