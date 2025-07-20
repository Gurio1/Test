namespace HCM.Features.Identity;

public sealed class JwtOptions
{
    public string JwtSecret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenMinutes { get; set; }
    public int RefreshTokenHours { get; set; }
}
