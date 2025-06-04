using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Noja.Application.Models.Common
{
    public class ServiceResponse<T>
    {
        //* Class provides consistent response format(All clients Web/Mobile) for profile service
        // Param T allows to return different types of data
        // Properties: Success status and Error messages

        
        
        public bool Success {get; set;}
        public string Message {get;set;} = string.Empty;
        public T? Data {get;set;}
        public IEnumerable<IdentityError> Errors {get;set;}
    }
}