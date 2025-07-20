namespace HCM.Domain.Identity.AccessTokens;

public sealed class AccessToken
{
    public required string Token { get; init; }
    public required DateTime TokenExpiryTime { get; init; }
}
