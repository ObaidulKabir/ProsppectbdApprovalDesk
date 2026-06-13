using ProspectbdApprovalDesk.Domain.Entities;

namespace ProspectbdApprovalDesk.Application.ActivityLogs;

public interface IActivityLogRepository
{
    Task<IReadOnlyList<ActivityLog>> ListAsync(int skip, int take, Guid? userId, string? entityName, CancellationToken ct);
    Task<long> CountAsync(Guid? userId, string? entityName, CancellationToken ct);
    Task AddAsync(ActivityLog log, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

