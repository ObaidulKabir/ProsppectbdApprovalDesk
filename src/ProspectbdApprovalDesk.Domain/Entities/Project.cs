using ProspectbdApprovalDesk.Domain.Common;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Domain.Entities;

public sealed class Project : AuditableEntityBase
{
    public string ProjectCode { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string? ProjectArea { get; set; }
    public string? ProjectLocation { get; set; }
    public string? DriveLink { get; set; }
    public string? ContactName { get; set; }
    public string? ContactNumber { get; set; }

    public string? Email { get; set; }
    public string? EmailPasswordEncrypted { get; set; }

    public string? EcpsAccountId { get; set; }
    public string? EcpsPasswordEncrypted { get; set; }
    public string? EcpsApplicationId { get; set; }

    public Guid? AssignedUserId { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    public DateOnly? SubmissionDate { get; set; }
    public DateOnly? ApprovalDate { get; set; }

    public string? Notes { get; set; }
}

