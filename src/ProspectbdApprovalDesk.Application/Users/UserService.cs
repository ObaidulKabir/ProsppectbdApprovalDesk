using AutoMapper;
using FluentValidation;
using ProspectbdApprovalDesk.Application.ActivityLogs;
using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Application.Exceptions;
using ProspectbdApprovalDesk.Application.Security;
using ProspectbdApprovalDesk.Application.Users.Dto;
using ProspectbdApprovalDesk.Domain.Entities;

namespace ProspectbdApprovalDesk.Application.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IActivityLogService _activity;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<UpdateUserRequest> _updateValidator;
    private readonly IValidator<ResetUserPasswordRequest> _resetPasswordValidator;

    public UserService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IActivityLogService activity,
        ICurrentUser currentUser,
        IMapper mapper,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator,
        IValidator<ResetUserPasswordRequest> resetPasswordValidator)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _activity = activity;
        _currentUser = currentUser;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _resetPasswordValidator = resetPasswordValidator;
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found.");
        return _mapper.Map<UserDto>(user);
    }

    public async Task<PagedResult<UserDto>> ListAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 20 : pageSize;

        var skip = (page - 1) * pageSize;

        var items = await _users.ListAsync(skip, pageSize, search, ct);
        var total = await _users.CountAsync(search, ct);

        return new PagedResult<UserDto>
        {
            Items = items.Select(_mapper.Map<UserDto>).ToArray(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var existing = await _users.GetByEmailAsync(request.Email.Trim(), ct);
        if (existing is not null)
            throw new AppException("Email is already in use.", 409);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Phone = request.Phone?.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Role = request.Role,
            IsActive = request.IsActive,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        await _activity.LogAsync(_currentUser.UserId, "UserCreated", "User", user.Id.ToString(), $"Created user {user.Email}.", ct);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var user = await _users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found.");

        user.FullName = request.FullName.Trim();
        user.Phone = request.Phone?.Trim();
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _users.SaveChangesAsync(ct);
        await _activity.LogAsync(_currentUser.UserId, "UserUpdated", "User", user.Id.ToString(), $"Updated user {user.Email}.", ct);

        return _mapper.Map<UserDto>(user);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found.");
        user.IsActive = false;
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _users.SaveChangesAsync(ct);
        await _activity.LogAsync(_currentUser.UserId, "UserDeactivated", "User", user.Id.ToString(), $"Deactivated user {user.Email}.", ct);
    }

    public async Task ResetPasswordAsync(Guid id, ResetUserPasswordRequest request, CancellationToken ct)
    {
        await _resetPasswordValidator.ValidateAndThrowAsync(request, ct);

        var user = await _users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found.");
        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        await _users.SaveChangesAsync(ct);
        await _activity.LogAsync(_currentUser.UserId, "UserPasswordReset", "User", user.Id.ToString(), $"Reset password for {user.Email}.", ct);
    }
}

