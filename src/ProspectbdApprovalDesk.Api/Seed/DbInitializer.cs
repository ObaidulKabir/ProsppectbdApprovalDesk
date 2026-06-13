using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProspectbdApprovalDesk.Application.Security;
using ProspectbdApprovalDesk.Domain.Entities;
using ProspectbdApprovalDesk.Domain.Enums;
using ProspectbdApprovalDesk.Infrastructure.Persistence;

namespace ProspectbdApprovalDesk.Api.Seed;

public static class DbInitializer
{
    public static async Task MigrateAndSeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await db.Database.MigrateAsync();

        var adminEmail = configuration["SeedAdmin:Email"]?.Trim().ToLowerInvariant();
        var adminPassword = configuration["SeedAdmin:Password"];
        var adminName = configuration["SeedAdmin:FullName"] ?? "System Admin";

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            return;

        var exists = await db.Users.AnyAsync(x => x.Email == adminEmail);
        if (exists) return;

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = adminName,
            Email = adminEmail,
            Phone = null,
            PasswordHash = hasher.HashPassword(adminPassword),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}

