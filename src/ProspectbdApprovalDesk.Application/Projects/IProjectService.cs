using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Application.Projects.Dto;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Projects;

public interface IProjectService
{
    Task<ProjectDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResult<ProjectListItemDto>> ListAsync(int page, int pageSize, string? search, Guid? assignedUserId, ProjectStatus? status, CancellationToken ct);
    Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken ct);
    Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectRequest request, CancellationToken ct);
    Task AssignAsync(Guid id, AssignProjectRequest request, CancellationToken ct);
    Task UpdateCredentialsAsync(Guid id, UpdateProjectCredentialsRequest request, CancellationToken ct);
    Task UpdateStatusAsync(Guid id, UpdateProjectStatusRequest request, CancellationToken ct);
    Task UpdateNotesAsync(Guid id, UpdateProjectNotesRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

