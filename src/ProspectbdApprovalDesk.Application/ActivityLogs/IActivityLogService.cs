using ProspectbdApprovalDesk.Application.ActivityLogs.Dto;
using ProspectbdApprovalDesk.Application.Common;

namespace ProspectbdApprovalDesk.Application.ActivityLogs;

public interface IActivityLogService
{
    Task<PagedResult<ActivityLogDto>> ListAsync(int page, int pageSize, Guid? userId, string? entityName, CancellationToken ct);
    Task LogAsync(Guid? userId, string action, string? entityName, string? entityId, string? description, CancellationToken ct);
}

