using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProspectbdApprovalDesk.Application.ActivityLogs;
using ProspectbdApprovalDesk.Application.Authentication;
using ProspectbdApprovalDesk.Application.Security;
using ProspectbdApprovalDesk.Application.Users;
using ProspectbdApprovalDesk.Application.Projects;
using ProspectbdApprovalDesk.Infrastructure.Authentication;
using ProspectbdApprovalDesk.Infrastructure.Persistence;
using ProspectbdApprovalDesk.Infrastructure.Repositories;
using ProspectbdApprovalDesk.Infrastructure.Security;

namespace ProspectbdApprovalDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        var encryptionKeyBase64 = configuration["Security:EncryptionKeyBase64"];
        if (string.IsNullOrWhiteSpace(encryptionKeyBase64))
            throw new InvalidOperationException("Security:EncryptionKeyBase64 is required.");

        services.AddSingleton<IEncryptionService>(_ => new AesGcmEncryptionService(encryptionKeyBase64));
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

        return services;
    }
}

