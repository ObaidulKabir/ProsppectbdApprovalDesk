using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProspectbdApprovalDesk.Application.ActivityLogs;
using ProspectbdApprovalDesk.Application.Common;

namespace ProspectbdApprovalDesk.Api.Controllers;

[ApiController]
[Route("api/activity-logs")]
[Authorize(Roles = "Admin")]
public sealed class ActivityLogsController : ControllerBase
{
    private readonly IActivityLogService _logs;

    public ActivityLogsController(IActivityLogService logs) => _logs = logs;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] Guid? userId = null, [FromQuery] string? entityName = null, CancellationToken ct = default)
    {
        var result = await _logs.ListAsync(page, pageSize, userId, entityName, ct);
        return Ok(ApiResponse<object>.Ok(result, null, HttpContext.TraceIdentifier));
    }
}

