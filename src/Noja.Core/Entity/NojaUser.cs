using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Noja.Core.Entity
{
    public abstract class NojaUser : IdentityUser
    {

        // Add a protected constructor
        protected NojaUser() { } 
               
        public UserType UserType {get; set;}

        //User properties
        public required string FirstName {get; set;}
        public required string LastName {get; set;}
        
        [NotMapped]
        public string Password {get; set;}

        [NotMapped]
        public string ConfirmPassword {get; set;}

    }

    public class Customer : NojaUser
    {
        public string State {get; set;}
        public string City {get; set;}
        public string SellerPhoneNumber {get; set;}
        public string StreetAddress {get; set;}
        public bool IsProfileComplete {get; set;} = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class Seller : NojaUser
    {
        
    }

    public enum UserType
    {
        Customer,
        Seller
    }
}