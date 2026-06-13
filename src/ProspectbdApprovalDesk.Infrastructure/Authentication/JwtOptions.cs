namespace ProspectbdApprovalDesk.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "ProspectbdApprovalDesk";
    public string Audience { get; init; } = "ProspectbdApprovalDesk";
    public string SigningKey { get; init; } = null!;
    public int AccessTokenMinutes { get; init; } = 720;
}

