using Microsoft.EntityFrameworkCore;
using ProspectbdApprovalDesk.Domain.Entities;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(x => x.Id);
            b.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Email).HasMaxLength(256).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.Phone).HasMaxLength(30);
            b.Property(x => x.PasswordHash).IsRequired();
            b.Property(x => x.Role).HasConversion<int>().IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt);
        });

        modelBuilder.Entity<Project>(b =>
        {
            b.ToTable("projects");
            b.HasKey(x => x.Id);
            b.Property(x => x.ProjectCode).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.ProjectCode).IsUnique();
            b.Property(x => x.ProjectName).HasMaxLength(200).IsRequired();
            b.Property(x => x.OwnerName).HasMaxLength(200).IsRequired();
            b.Property(x => x.ProjectArea).HasMaxLength(200);
            b.Property(x => x.ProjectLocation).HasMaxLength(500);
            b.Property(x => x.DriveLink).HasMaxLength(2048);
            b.Property(x => x.ContactName).HasMaxLength(200);
            b.Property(x => x.ContactNumber).HasMaxLength(30);
            b.Property(x => x.Email).HasMaxLength(256);
            b.Property(x => x.EmailPasswordEncrypted).HasMaxLength(2048);
            b.Property(x => x.EcpsAccountId).HasMaxLength(100);
            b.Property(x => x.EcpsPasswordEncrypted).HasMaxLength(2048);
            b.Property(x => x.EcpsApplicationId).HasMaxLength(100);
            b.Property(x => x.AssignedUserId);
            b.Property(x => x.Status).HasConversion<int>().IsRequired();
            b.Property(x => x.SubmissionDate);
            b.Property(x => x.ApprovalDate);
            b.Property(x => x.Notes).HasMaxLength(4000);
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt);
        });

        modelBuilder.Entity<ActivityLog>(b =>
        {
            b.ToTable("activity_logs");
            b.HasKey(x => x.Id);
            b.Property(x => x.UserId);
            b.Property(x => x.Action).HasMaxLength(100).IsRequired();
            b.Property(x => x.EntityName).HasMaxLength(100);
            b.Property(x => x.EntityId).HasMaxLength(100);
            b.Property(x => x.Timestamp).IsRequired();
            b.Property(x => x.Description).HasMaxLength(1000);
            b.HasIndex(x => x.Timestamp);
            b.HasIndex(x => x.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}

