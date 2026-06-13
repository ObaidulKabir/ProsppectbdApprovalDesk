using ProspectbdApprovalDesk.Domain.Entities;

namespace ProspectbdApprovalDesk.Application.Authentication;

public interface IJwtTokenService
{
    (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(User user);
}

