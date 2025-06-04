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
             // Find the startup project directory (where appsettings.json is located)
            // This assumes you're running from the solution root or the API project
            string projectDir = Directory.GetCurrentDirectory();
            
            // Try to find appsettings.json by going up directories if needed
            string configPath = Path.Combine(projectDir, "appsettings.json");
            if (!File.Exists(configPath))
            {
                // Try looking in the API project directory
                configPath = Path.Combine(projectDir, "..", "Noja.API", "appsettings.json");
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Could not find appsettings.json in {projectDir} or parent directories");
                }
                
                projectDir = Path.GetDirectoryName(configPath);
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectDir)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DbConnectionString");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'");
            }

            var optionsBuilder = new DbContextOptionsBuilder<NojaDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new NojaDbContext(optionsBuilder.Options);
        }
        
    }
}