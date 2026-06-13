using ProspectbdApprovalDesk.Application.Common;
using ProspectbdApprovalDesk.Application.Users.Dto;

namespace ProspectbdApprovalDesk.Application.Users;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResult<UserDto>> ListAsync(int page, int pageSize, string? search, CancellationToken ct);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken ct);
    Task DeactivateAsync(Guid id, CancellationToken ct);
    Task ResetPasswordAsync(Guid id, ResetUserPasswordRequest request, CancellationToken ct);
}

