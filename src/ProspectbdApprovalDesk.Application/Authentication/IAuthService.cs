using ProspectbdApprovalDesk.Application.Authentication.Dto;

namespace ProspectbdApprovalDesk.Application.Authentication;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
}

