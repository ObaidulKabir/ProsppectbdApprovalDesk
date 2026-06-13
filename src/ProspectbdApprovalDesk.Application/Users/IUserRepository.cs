using ProspectbdApprovalDesk.Domain.Entities;

namespace ProspectbdApprovalDesk.Application.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<IReadOnlyList<User>> ListAsync(int skip, int take, string? search, CancellationToken ct);
    Task<long> CountAsync(string? search, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

