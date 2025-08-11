using Noja.Infrastructure;  
using Noja.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;
using Noja.Application.Services.Auth;
using System.IdentityModel.Tokens.Jwt;
using Noja.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Infrastructure services

// Load environment-based config (Development / Production)
var configuration = builder.Configuration;

builder.Services.AddInfrastructure(builder.Configuration,builder.Environment);

//Application services
builder.Services.AddApplication();

// Authentication service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        ValidAudience = builder.Configuration["Jwt:ValidAudience"], 
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

            var logoutService = context.HttpContext
            .RequestServices.GetRequiredService<ILogoutService>();

            var token = context.SecurityToken as JwtSecurityToken;

            var rawToken = context.HttpContext.Request.Headers["Authorization"]
                    .ToString().Replace("Bearer ", "");
            // logger.LogInformation("Validating token in JWT middleware");

           
            if (!string.IsNullOrEmpty(rawToken))
            {
                var isBlacklisted = await logoutService.IsTokenBlacklistedAsync(rawToken);
                // logger.LogInformation($"Token blacklist check result: {isBlacklisted}");
                
                if (isBlacklisted)
                {
                    // logger.LogWarning("Rejecting blacklisted token");
                    context.Fail("Token has been revoked");
                }
            }
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt => {
        opt.Title = "NojaAPI";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
