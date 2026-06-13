using ProspectbdApprovalDesk.Domain.Common;

namespace ProspectbdApprovalDesk.Domain.Entities;

public sealed class ActivityLog : EntityBase
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = null!;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? Description { get; set; }
}

