using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Application.Projects;
using ProspectbdApprovalDesk.Application.Projects.Dto;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectService _projects;

    public ProjectsController(IProjectService projects) => _projects = projects;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] Guid? assignedUserId = null, [FromQuery] ProjectStatus? status = null, CancellationToken ct = default)
    {
        var result = await _projects.ListAsync(page, pageSize, search, assignedUserId, status, ct);
        return Ok(ApiResponse<object>.Ok(result, null, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Get(Guid id, CancellationToken ct)
    {
        var result = await _projects.GetByIdAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(result, null, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var result = await _projects.CreateAsync(request, ct);
        return Ok(ApiResponse<object>.Ok(result, "Project created.", HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken ct)
    {
        var result = await _projects.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(result, "Project updated.", HttpContext.TraceIdentifier));
    }

    [HttpPost("{id:guid}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Assign(Guid id, [FromBody] AssignProjectRequest request, CancellationToken ct)
    {
        await _projects.AssignAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Assignment updated.", HttpContext.TraceIdentifier));
    }

    [HttpPatch("{id:guid}/credentials")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCredentials(Guid id, [FromBody] UpdateProjectCredentialsRequest request, CancellationToken ct)
    {
        await _projects.UpdateCredentialsAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Credentials updated.", HttpContext.TraceIdentifier));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(Guid id, [FromBody] UpdateProjectStatusRequest request, CancellationToken ct)
    {
        await _projects.UpdateStatusAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Status updated.", HttpContext.TraceIdentifier));
    }

    [HttpPatch("{id:guid}/notes")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateNotes(Guid id, [FromBody] UpdateProjectNotesRequest request, CancellationToken ct)
    {
        await _projects.UpdateNotesAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Notes updated.", HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _projects.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Project deleted.", HttpContext.TraceIdentifier));
    }
}

