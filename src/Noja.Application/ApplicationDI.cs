using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Noja.Application.Services.Admin;
using Noja.Application.Services.Admin.Interface;
using Noja.Application.Services.Auth;
using Noja.Application.Services.Products;
using Noja.Application.Services.TeamManagement;
using Noja.Application.Services.TeamManagement.Interface;
using Noja.Core.Interfaces.Service;


namespace Noja.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthServices>();
            services.AddMemoryCache();
            services.AddScoped<ILogoutService, LogoutService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IProductService, ProductService>();

            // ====== team management services ====== //
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IContributionService, ContributionService>();

            return services;
        }
    }
}

