using System.Security.Cryptography;

namespace HCM.Domain.Identity.RefreshTokens;

public sealed class RefreshToken
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid PersonId { get; init; }
    public required string Token { get; init; }
    public DateTime ExpiresAt { get; init; }
    
    private RefreshToken(){}
    
    public static RefreshToken Create(Guid userId,int refreshTokenHours) =>
        new()
        {
            PersonId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddHours(refreshTokenHours)
        };
}
