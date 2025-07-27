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

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Role).HasConversion<string>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Manken entity configuration
            modelBuilder.Entity<Manken>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.BirthDate).IsRequired();
                entity.Property(e => e.Gender).HasConversion<string>();
                entity.Property(e => e.Height).IsRequired();
                entity.Property(e => e.Weight).IsRequired();
                entity.Property(e => e.HairColor).HasMaxLength(50);
                entity.Property(e => e.EyeColor).HasMaxLength(50);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.Category).HasConversion<string>();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);
                entity.Property(e => e.PhotoPath).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Organization entity configuration
            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.TotalBudget).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<string>().HasDefaultValue(OrganizationStatus.Planned);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // CreatedBy relationship
                entity.HasOne(e => e.CreatedBy)
                    .WithMany(e => e.CreatedOrganizations)
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Assignment entity configuration
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.Property(e => e.MankenId).IsRequired();
                entity.Property(e => e.OrganizationId).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.Fee).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<string>().HasDefaultValue(AssignmentStatus.Scheduled);
                entity.Property(e => e.DailyRate).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.NumberOfDays).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.IncludesMeal).HasDefaultValue(false);
                entity.Property(e => e.IncludesAccommodation).HasDefaultValue(false);
                entity.Property(e => e.MealCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AccommodationCost).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPayment).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Manken relationship
                entity.HasOne(e => e.Manken)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.MankenId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Organization relationship
                entity.HasOne(e => e.Organization)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CreatedBy relationship
                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Check constraint for date validation
                entity.HasCheckConstraint("CK_Assignment_DateRange", "EndTime > StartTime");
            });

            // Payment entity configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.OrganizationId).IsRequired();
                entity.Property(e => e.PaymentType).HasConversion<string>();
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).HasConversion<string>().HasDefaultValue(PaymentStatus.Pending);
                entity.Property(e => e.PaymentDate).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Organization relationship
                entity.HasOne(e => e.Organization)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Assignment relationship
                entity.HasOne(e => e.Assignment)
                    .WithMany()
                    .HasForeignKey(e => e.AssignmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ProcessedBy relationship
                entity.HasOne(e => e.ProcessedBy)
                    .WithMany(e => e.ProcessedPayments)
                    .HasForeignKey(e => e.ProcessedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // CreatedBy relationship
                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Indexes for better performance
            modelBuilder.Entity<Manken>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Manken>()
                .HasIndex(e => new { e.IsActive, e.IsAvailable });

            modelBuilder.Entity<Organization>()
                .HasIndex(e => e.Date);

            modelBuilder.Entity<Assignment>()
                .HasIndex(e => new { e.MankenId, e.StartTime, e.EndTime });

            modelBuilder.Entity<Payment>()
                .HasIndex(e => e.PaymentDate);
        }
    }
} 