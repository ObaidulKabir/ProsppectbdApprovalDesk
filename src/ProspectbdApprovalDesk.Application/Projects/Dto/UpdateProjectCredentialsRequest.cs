namespace ProspectbdApprovalDesk.Application.Projects.Dto;

public sealed record UpdateProjectCredentialsRequest(
    string? EmailPassword,
    string? EcpsPassword);

