namespace ProspectbdApprovalDesk.Domain.Common;

public abstract class AuditableEntityBase : EntityBase
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

