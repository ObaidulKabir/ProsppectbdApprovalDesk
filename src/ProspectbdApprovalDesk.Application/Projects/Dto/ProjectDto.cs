using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Projects.Dto;

public sealed record ProjectDto(
    Guid Id,
    string ProjectCode,
    string ProjectName,
    string OwnerName,
    string? ProjectArea,
    string? ProjectLocation,
    string? DriveLink,
    string? ContactName,
    string? ContactNumber,
    string? Email,
    string? EmailPassword,
    string? EcpsAccountId,
    string? EcpsPassword,
    string? EcpsApplicationId,
    Guid? AssignedUserId,
    ProjectStatus Status,
    DateOnly? SubmissionDate,
    DateOnly? ApprovalDate,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

