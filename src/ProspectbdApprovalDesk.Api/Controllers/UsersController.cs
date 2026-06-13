using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Application.Users;
using ProspectbdApprovalDesk.Application.Users.Dto;

namespace ProspectbdApprovalDesk.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users) => _users = users;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        var result = await _users.ListAsync(page, pageSize, search, ct);
        return Ok(ApiResponse<object>.Ok(result, null, HttpContext.TraceIdentifier));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Get(Guid id, CancellationToken ct)
    {
        var result = await _users.GetByIdAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(result, null, HttpContext.TraceIdentifier));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await _users.CreateAsync(request, ct);
        return Ok(ApiResponse<object>.Ok(result, "User created.", HttpContext.TraceIdentifier));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await _users.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(result, "User updated.", HttpContext.TraceIdentifier));
    }

    [HttpPost("{id:guid}/reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword(Guid id, [FromBody] ResetUserPasswordRequest request, CancellationToken ct)
    {
        await _users.ResetPasswordAsync(id, request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Password reset.", HttpContext.TraceIdentifier));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Deactivate(Guid id, CancellationToken ct)
    {
        await _users.DeactivateAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "User deactivated.", HttpContext.TraceIdentifier));
    }
}

