namespace ProspectbdApprovalDesk.Application.Projects.Dto;

public sealed record UpdateProjectRequest(
    string ProjectName,
    string OwnerName,
    string? ProjectArea,
    string? ProjectLocation,
    string? DriveLink,
    string? ContactName,
    string? ContactNumber,
    string? Email,
    string? EcpsAccountId,
    string? EcpsApplicationId,
    string? Notes);

