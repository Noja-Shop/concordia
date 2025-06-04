using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.API.Endpoints.AuthEndpoints
{
    public static class AuthAPIEndpoints
    {
        private const string ApiBase = "api";

        public static class Customer
        {
            private const string Base = $"{ApiBase}/user";
            public const string CustomerSignup = $"{Base}/signup";
            public const string CustomerLogin = $"{Base}/login";
            public const string CustomerLogout = $"{Base}/logout";
            public const string CustomerProfile = $"{Base}/profile";
            public const string CustomerUpdateProfile = $"{Base}/profile/update";
        }

        public static class Seller
        {
            private const string Base = $"{ApiBase}/admin";
            public const string AdminSignup = $"{Base}/signup";
            public const string AdminLogin = $"{Base}/login";
        }
    }
}