namespace ProspectbdApprovalDesk.Application.Projects.Dto;

public sealed record CreateProjectRequest(
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
    string? Notes);

