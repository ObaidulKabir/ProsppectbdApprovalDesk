using Microsoft.EntityFrameworkCore;
using ProspectbdApprovalDesk.Application.ActivityLogs;
using ProspectbdApprovalDesk.Domain.Entities;
using ProspectbdApprovalDesk.Infrastructure.Persistence;

namespace ProspectbdApprovalDesk.Infrastructure.Repositories;

public sealed class ActivityLogRepository : IActivityLogRepository
{
    private readonly AppDbContext _db;

    public ActivityLogRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ActivityLog>> ListAsync(int skip, int take, Guid? userId, string? entityName, CancellationToken ct)
    {
        var query = _db.ActivityLogs.AsNoTracking().AsQueryable();

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(x => x.EntityName == entityName);

        return await query
            .OrderByDescending(x => x.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<long> CountAsync(Guid? userId, string? entityName, CancellationToken ct)
    {
        var query = _db.ActivityLogs.AsNoTracking().AsQueryable();

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(x => x.EntityName == entityName);

        return await query.LongCountAsync(ct);
    }

    public Task AddAsync(ActivityLog log, CancellationToken ct) => _db.ActivityLogs.AddAsync(log, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

