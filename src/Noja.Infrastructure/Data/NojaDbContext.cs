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
        public DbSet<Team> Teams {get; set;}
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TeamMember> Members { get; set; }
        public DbSet<Contribution> Contributions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the inheritance strategy
            modelBuilder.Entity<NojaUser>()
                .HasDiscriminator<string>("UserTypeName")
                .HasValue<Seller>(UserType.Seller.ToString())
                .HasValue<Customer>(UserType.Customer.ToString());

            // Contribution entity
            modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Team)
            .WithMany(t => t.Contributions)
            .HasForeignKey(t => t.TeamId);

            modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId);

            modelBuilder.Entity<Contribution>()
            .HasOne(c => c.Payment)
            .WithMany()
            .HasForeignKey(c => c.PaymentId);

            // configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Category)
                .HasConversion<string>();
                entity.Property(e => e.PackageType)
                .HasConversion<string>();
                entity.Property(e => e.PackagePrice)
                .HasColumnType("decimal(18,2)");
                entity.Property(e => e.PackageSize)
                .HasColumnType("decimal(10,2)");
                entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10,2)");
                entity.Property(e => e.ContainerSize)
                .HasColumnType("decimal(10,3)");
                entity.Property(e => e.ContainerType)
                .HasConversion<string>();
                entity.Property(e => e.MeasurementUnit)
                .HasConversion<string>();
                entity.Property(e => e.ContainerCount)
                .HasColumnType("int")
                .HasDefaultValue(1);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new
                {
                    e.Category,
                    e.IsActive
                });

                // ==== Team Configuration ==== //
                modelBuilder.Entity<Team>(entity =>
                {
                    entity.Property(e => e.Status).HasConversion<string>();
                    entity.Property(e => e.TargetQuantity).HasColumnType("decimal(10,3)");
                    entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)");
                    entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

                    entity.HasIndex(e => e.Status);
                    entity.HasIndex(e => e.CreatedAt);
                    entity.HasIndex(e => new { e.Status, e.CreatedAt });

                    // Relationships
                    entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Creator)
                    .WithMany(c => c.CreatedTeams)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                });

                // ===== Team Member Configuration ==== //
                modelBuilder.Entity<TeamMember>(entity =>
                {
                    entity.Property(e => e.Quantity).HasColumnType("decimal(10,3)");
                    entity.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)");

                    entity.HasIndex(e => e.TeamId);
                    entity.HasIndex(e => e.CustomerId);
                    entity.HasIndex(e => new { e.TeamId, e.CustomerId }).IsUnique();

                    // Relationships
                    entity.HasOne(e => e.Team)
                    .WithMany(t => t.Members)
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Customer)
                    .WithMany(c => c.TeamMemberships)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                });

                // ===== Payment configuration ===== //
                modelBuilder.Entity<Payment>(entity =>
                {
                    entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                    entity.Property(e => e.PaymentMethod).HasConversion<string>();
                    entity.Property(e => e.Status).HasConversion<string>();

                    entity.HasIndex(e => e.Status);
                    entity.HasIndex(e => e.CreatedAt);
                    entity.HasIndex(e => new { e.CustomerId, e.Status });

                    // Relationships
                    entity
                    .HasOne(e => e.Customer)
                    .WithMany(c => c.Payments)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Team)
                    .WithMany()
                    .HasForeignKey(e => e.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(t => t.Contribution)
                    .WithOne(c => c.Payment)
                    .HasForeignKey<Contribution>(c => c.PaymentId);
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