using System.Security.Claims;
using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Domain.Enums;

namespace ProspectbdApprovalDesk.Api.Security;

public sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var role = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("role");

            return Enum.TryParse<UserRole>(role, out var parsed) ? parsed : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
}

