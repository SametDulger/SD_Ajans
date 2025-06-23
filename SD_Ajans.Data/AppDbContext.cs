using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SD_Ajans.Core.Entities;

namespace SD_Ajans.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Manken> Mankens { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Manken>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Mankens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasOne(e => e.CreatedBy)
                    .WithMany(e => e.CreatedOrganizations)
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasOne(e => e.Manken)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.MankenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Organization)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(e => e.Organization)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Assignment)
                    .WithMany()
                    .HasForeignKey(e => e.AssignmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProcessedBy)
                    .WithMany()
                    .HasForeignKey(e => e.ProcessedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
} 