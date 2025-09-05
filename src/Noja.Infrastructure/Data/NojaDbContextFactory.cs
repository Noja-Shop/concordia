using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Configuration.FileExtensions;
// using Microsoft.Extensions.Configuration.Json;

namespace Noja.Infrastructure.Data
{
    public class NojaDbContextFactory : IDesignTimeDbContextFactory<NojaDbContext>
    {
        public NojaDbContext CreateDbContext(string[] args)
        {
            // This factory is used by `dotnet ef` CLI tools. It builds a configuration
            // that mirrors the runtime setup to get the connection string. It assumes the
            // command is run with Noja.API as the startup project, which sets the
            // current directory correctly.
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnectionString");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'DefaultConnectionString'. " +
                    "Check your appsettings.json, environment-specific settings, or environment variables.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<NojaDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new NojaDbContext(optionsBuilder.Options);
        }
        
    }
}