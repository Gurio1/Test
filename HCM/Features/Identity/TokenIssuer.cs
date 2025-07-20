using HCM.Domain.Identity.AccessTokens;
using HCM.Domain.Identity.RefreshTokens;
using HCM.Domain.Persons;
using HCM.Features.Identity.Contracts;
using HCM.Infrastructure;
using Microsoft.Extensions.Options;

namespace HCM.Features.Identity;

public sealed class TokenIssuer
{
    private readonly JwtTokenGenerator jwtTokenGenerator;
    private readonly ApplicationDbContext context;
    private readonly JwtOptions options;
    
    public TokenIssuer(JwtTokenGenerator jwtTokenGenerator, ApplicationDbContext context,IOptions<JwtOptions> options)
    {
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.context = context;
        this.options = options.Value;
    }
    
    public async Task<TokenPair> IssueNewTokensAsync(
        Person person,
        RefreshToken? previousToken,
        CancellationToken ct = default)
    {
        var accessToken = jwtTokenGenerator.GenerateAccessToken(person);
        
        var refreshToken = RefreshToken.Create(person.Id,options.RefreshTokenHours);
        context.RefreshTokens.Add(refreshToken);
        
        if (previousToken is not null)
            context.RefreshTokens.Remove(previousToken);
        
        await context.SaveChangesAsync(ct);
        
        return new TokenPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}

public class TokenPair
{
    public AccessToken AccessToken { get; init; }
    public RefreshToken RefreshToken { get; init; }
}

public static class Mapper
{
    
    public static AuthResponse ToAuthResponse(this TokenPair tokenPair) =>
        new()
        {
            AccessToken = tokenPair.AccessToken.Token,
            AccessExpiry = tokenPair.AccessToken.TokenExpiryTime,
            RefreshExpiry = tokenPair.RefreshToken.ExpiresAt
        };
}
