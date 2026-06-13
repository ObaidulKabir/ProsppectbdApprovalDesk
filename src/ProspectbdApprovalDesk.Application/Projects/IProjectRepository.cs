using ProspectbdApprovalDesk.Domain.Entities;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Projects;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Project>> ListAsync(int skip, int take, string? search, Guid? assignedUserId, ProjectStatus? status, CancellationToken ct);
    Task<long> CountAsync(string? search, Guid? assignedUserId, ProjectStatus? status, CancellationToken ct);
    Task AddAsync(Project project, CancellationToken ct);
    Task DeleteAsync(Project project, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

