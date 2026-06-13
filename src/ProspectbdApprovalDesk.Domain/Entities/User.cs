using ProspectbdApprovalDesk.Domain.Common;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Domain.Entities;

public sealed class User : AuditableEntityBase
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}

