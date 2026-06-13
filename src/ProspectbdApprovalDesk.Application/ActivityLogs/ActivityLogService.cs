using AutoMapper;
using ProspectbdApprovalDesk.Application.ActivityLogs.Dto;
using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Domain.Entities;

namespace ProspectbdApprovalDesk.Application.ActivityLogs;

public sealed class ActivityLogService : IActivityLogService
{
    private readonly IActivityLogRepository _logs;
    private readonly IMapper _mapper;

    public ActivityLogService(IActivityLogRepository logs, IMapper mapper)
    {
        _logs = logs;
        _mapper = mapper;
    }

    public async Task<PagedResult<ActivityLogDto>> ListAsync(int page, int pageSize, Guid? userId, string? entityName, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

        var skip = (page - 1) * pageSize;
        var items = await _logs.ListAsync(skip, pageSize, userId, entityName, ct);
        var total = await _logs.CountAsync(userId, entityName, ct);

        return new PagedResult<ActivityLogDto>
        {
            Items = items.Select(_mapper.Map<ActivityLogDto>).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task LogAsync(Guid? userId, string action, string? entityName, string? entityId, string? description, CancellationToken ct)
    {
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Timestamp = DateTimeOffset.UtcNow,
            Description = description
        };

        await _logs.AddAsync(log, ct);
        await _logs.SaveChangesAsync(ct);
    }
}

