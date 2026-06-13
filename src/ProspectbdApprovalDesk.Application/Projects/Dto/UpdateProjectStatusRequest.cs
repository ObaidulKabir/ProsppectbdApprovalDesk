using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Projects.Dto;

public sealed record UpdateProjectStatusRequest(
    ProjectStatus Status,
    DateOnly? SubmissionDate,
    DateOnly? ApprovalDate);

