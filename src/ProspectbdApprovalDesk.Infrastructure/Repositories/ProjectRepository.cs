using Microsoft.EntityFrameworkCore;
using ProspectbdApprovalDesk.Application.Projects;
using ProspectbdApprovalDesk.Domain.Entities;
using ProspectbdApprovalDesk.Domain.Enums;
using ProspectbdApprovalDesk.Infrastructure.Persistence;

namespace ProspectbdApprovalDesk.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;

    public ProjectRepository(AppDbContext db) => _db = db;

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Projects.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Project>> ListAsync(int skip, int take, string? search, Guid? assignedUserId, ProjectStatus? status, CancellationToken ct)
    {
        var query = _db.Projects.AsNoTracking().AsQueryable();

        if (assignedUserId.HasValue)
            query = query.Where(x => x.AssignedUserId == assignedUserId);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.ProjectCode.ToLower()!.Contains(term) ||
                x.ProjectName.ToLower()!.Contains(term) ||
                x.OwnerName.ToLower()!.Contains(term));
        }

        return await query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<long> CountAsync(string? search, Guid? assignedUserId, ProjectStatus? status, CancellationToken ct)
    {
        var query = _db.Projects.AsNoTracking().AsQueryable();

        if (assignedUserId.HasValue)
            query = query.Where(x => x.AssignedUserId == assignedUserId);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.ProjectCode.ToLower()!.Contains(term) ||
                x.ProjectName.ToLower()!.Contains(term) ||
                x.OwnerName.ToLower()!.Contains(term));
        }

        return await query.LongCountAsync(ct);
    }

    public Task AddAsync(Project project, CancellationToken ct) => _db.Projects.AddAsync(project, ct).AsTask();

    public Task DeleteAsync(Project project, CancellationToken ct)
    {
        _db.Projects.Remove(project);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

