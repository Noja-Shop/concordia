using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Noja.Core.Entity;
using Noja.Core.Interfaces.Repository;
using Noja.Core.Interfaces.Repository.Teams;
using Noja.Infrastructure.Authentication;
using Noja.Infrastructure.Data;
using Noja.Infrastructure.Options;
using Noja.Infrastructure.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;




namespace Noja.Infrastructure
{
    public static class InfrastrutureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration,IWebHostEnvironment env)
        {
            services.Configure<JwtOption>(options => configuration.GetSection(JwtOption.JwtOptionKey));
            // services.AddDbContext<NojaDbContext>(options => options.
            // UseNpgsql(configuration.GetConnectionString("DbConnectionString")));

             var connStr = configuration.GetConnectionString("DbConnectionString")
                  ?? configuration["ConnectionStrings:DbConnectionString"];

            // var certPath = Path.Combine(contentRootPath, "BaltimoreCyberTrustRoot.crt.pem");

            // services.AddDbContext<NojaDbContext>(options =>
            //     options.UseNpgsql($"{connStr};Ssl Mode=Require;Trust Server Certificate=true;Ssl Root Cert={certPath}")
            // );

            if (env.IsDevelopment())
            {
                // Local DB without SSL
                services.AddDbContext<NojaDbContext>(options =>
                    options.UseNpgsql(connStr));
            }
            else
            {
                // Azure DB with SSL cert
                var certPath = Path.Combine(env.ContentRootPath, "BaltimoreCyberTrustRoot.crt.pem");
                services.AddDbContext<NojaDbContext>(options =>
                    options.UseNpgsql($"{connStr};Ssl Mode=Require;Trust Server Certificate=true;Ssl Root Cert={certPath}"));
            }

            services.AddIdentityCore<NojaUser>(options => 
            {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                    
                    
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters 
                    = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddEntityFrameworkStores<NojaDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<NojaUser>>();

            services.AddScoped<ITokenService, JwtTokenGenerator>();
            services.AddScoped<IProductRepository, ProductRepository>();

            /// ====== team management services ====== ///
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IContributionRepository, ContributionRepository>();

            return services;
        }
    }
}