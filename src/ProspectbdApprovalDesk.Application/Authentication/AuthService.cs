using FluentValidation;
using ProspectbdApprovalDesk.Application.ActivityLogs;
using ProspectbdApprovalDesk.Application.Authentication.Dto;
using ProspectbdApprovalDesk.Application.Exceptions;
using ProspectbdApprovalDesk.Application.Security;
using ProspectbdApprovalDesk.Application.Users;

namespace ProspectbdApprovalDesk.Application.Authentication;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IActivityLogService _activity;
    private readonly IValidator<LoginRequest> _validator;

    public AuthService(
        IUserRepository users,
        IJwtTokenService jwt,
        IPasswordHasher passwordHasher,
        IActivityLogService activity,
        IValidator<LoginRequest> validator)
    {
        _users = users;
        _jwt = jwt;
        _passwordHasher = passwordHasher;
        _activity = activity;
        _validator = validator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request, ct);

        var user = await _users.GetByEmailAsync(request.Email.Trim(), ct);
        if (user is null || !user.IsActive)
            throw new UnauthorizedException("Invalid credentials.");

        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, request.Password))
            throw new UnauthorizedException("Invalid credentials.");

        var (token, expiresAt) = _jwt.CreateAccessToken(user);

        await _activity.LogAsync(user.Id, "Login", "User", user.Id.ToString(), "User logged in.", ct);

        return new LoginResponse(token, expiresAt, user.Id, user.FullName, user.Email, user.Role);
    }
}

