using Microsoft.EntityFrameworkCore;
using ProspectbdApprovalDesk.Application.Users;
using ProspectbdApprovalDesk.Domain.Entities;
using ProspectbdApprovalDesk.Infrastructure.Persistence;

namespace ProspectbdApprovalDesk.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _db.Users.FirstOrDefaultAsync(x => x.Email == normalized, ct);
    }

    public async Task<IReadOnlyList<User>> ListAsync(int skip, int take, string? search, CancellationToken ct)
    {
        var query = _db.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.Email.ToLower()!.Contains(term) || x.FullName.ToLower()!.Contains(term));
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<long> CountAsync(string? search, CancellationToken ct)
    {
        var query = _db.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(x => x.Email.ToLower()!.Contains(term) || x.FullName.ToLower()!.Contains(term));
        }

        return await query.LongCountAsync(ct);
    }

    public Task AddAsync(User user, CancellationToken ct) => _db.Users.AddAsync(user, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

