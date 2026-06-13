using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProspectbdApprovalDesk.Application.Authentication;
using ProspectbdApprovalDesk.Application.Authentication.Dto;
using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;

    public AuthController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(request, ct);
        return Ok(ApiResponse<LoginResponse>.Ok(result, "Login successful.", HttpContext.TraceIdentifier));
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponse<object>> Me()
    {
        var payload = new
        {
            UserId = _currentUser.UserId,
            Role = _currentUser.Role
        };

        return Ok(ApiResponse<object>.Ok(payload, null, HttpContext.TraceIdentifier));
    }
}

