using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Application.Users.Dto;

public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string? Phone,
    string Password,
    UserRole Role,
    bool IsActive);

