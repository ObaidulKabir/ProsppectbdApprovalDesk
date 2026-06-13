using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Authentication.Dto;

public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    string FullName,
    string Email,
    UserRole Role);

