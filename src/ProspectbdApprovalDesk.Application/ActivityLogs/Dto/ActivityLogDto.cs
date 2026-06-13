namespace ProspectbdApprovalDesk.Application.ActivityLogs.Dto;

public sealed record ActivityLogDto(
    Guid Id,
    Guid? UserId,
    string Action,
    string? EntityName,
    string? EntityId,
    DateTimeOffset Timestamp,
    string? Description);

