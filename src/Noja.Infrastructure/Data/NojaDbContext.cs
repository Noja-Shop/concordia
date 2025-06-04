using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Noja.Core.Entity;


namespace Noja.Infrastructure.Data
{
    public class NojaDbContext : IdentityDbContext<NojaUser>
    {
        public NojaDbContext(DbContextOptions<NojaDbContext> options): base(options)
        {
            
        }

        //DbSets for concrete implementations
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure the inheritance strategy
            modelBuilder.Entity<NojaUser>()
                .HasDiscriminator<string>("UserTypeName")
                .HasValue<Seller>(UserType.Seller.ToString())
                .HasValue<Customer>(UserType.Customer.ToString());

            // configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Category)
                .HasConversion<string>();
                entity.Property(e => e.UnitOfMeasure)
                .HasConversion<string>();
                entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)");
                entity.Property(e => e.UnitSize)
                .HasColumnType("decimal(10,2)");

                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.Category, e.IsActive 
            });

            //configure relationships
            entity.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

            });
        }
    }
}