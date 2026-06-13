using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Projects.Dto;

public sealed record ProjectListItemDto(
    Guid Id,
    string ProjectCode,
    string ProjectName,
    string OwnerName,
    Guid? AssignedUserId,
    ProjectStatus Status,
    DateOnly? SubmissionDate,
    DateOnly? ApprovalDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

