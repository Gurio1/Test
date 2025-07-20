using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HCM.Domain.Persons;
using HCM.Features.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HCM.Domain.Identity.AccessTokens;

public sealed class JwtTokenGenerator
{
    private readonly JwtOptions options;
    
    public JwtTokenGenerator(IOptions<JwtOptions> options) => this.options = options.Value;
    
    public AccessToken GenerateAccessToken(Person person)
    {
        var claims = new[]
        {
            new Claim(ApplicationClaims.PersonId, person.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, person.Email),
            new Claim(ClaimTypes.Role, person.Role),
            new Claim(ApplicationClaims.Department, person.Department)
        };
        
        var expiryTime = DateTime.UtcNow.AddMinutes(options.AccessTokenMinutes);
        
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires:expiryTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.JwtSecret)),
                SecurityAlgorithms.HmacSha256
            )
        );
        
        string tokenStr =  new JwtSecurityTokenHandler().WriteToken(token);
        
        return new AccessToken() { Token = tokenStr, TokenExpiryTime = expiryTime };
    }
}
