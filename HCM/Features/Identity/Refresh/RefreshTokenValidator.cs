using HCM.Domain.Identity.RefreshTokens;

namespace HCM.Features.Identity.Refresh;

public static class RefreshTokenValidator
{
    public static bool IsRefreshTokenValid(RefreshToken? token) => !(token is null || token.ExpiresAt < DateTime.UtcNow);
}
