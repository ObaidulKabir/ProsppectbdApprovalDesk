using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Users.Dto;

public sealed record UpdateUserRequest(
    string FullName,
    string? Phone,
    UserRole Role,
    bool IsActive);

