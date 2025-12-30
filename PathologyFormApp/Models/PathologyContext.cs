using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PathologyFormApp.Models
{
    public class PathologyContext(DbContextOptions<PathologyContext> options) : IdentityDbContext<User>(options)
    {

        public DbSet<PathologyRequisitionForm> PathologyRequisitionForms { get; set; } = null!;
        public DbSet<FormHistory> FormHistory { get; set; } = null!;
        public DbSet<SpecimenType> SpecimenTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PathologyRequisitionForm>().ToTable("PathologyForm");
            modelBuilder.Entity<SpecimenType>().ToTable("SpecimenTypes");

            // Configure Status property
            modelBuilder.Entity<PathologyRequisitionForm>()
                .Property(p => p.Status)
                .HasConversion<string>();

            // Configure relationships for PathologyRequisitionForm
            modelBuilder.Entity<PathologyRequisitionForm>()
                .HasOne(f => f.CreatedBy)
                .WithMany(u => u.CreatedForms)
                .HasForeignKey(f => f.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<PathologyRequisitionForm>()
                .HasOne(f => f.ReviewedBy)
                .WithMany()
                .HasForeignKey(f => f.ReviewedById)
                .IsRequired(false) // ReviewedBy can be null
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure relationships for FormHistory
            modelBuilder.Entity<FormHistory>()
                .HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            modelBuilder.Entity<FormHistory>()
                .HasOne(h => h.Form)
                .WithMany(f => f.FormHistory)
                .HasForeignKey(h => h.FormUHID)
                .OnDelete(DeleteBehavior.Cascade); // If form is deleted, history should be too

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
        }
    }
}