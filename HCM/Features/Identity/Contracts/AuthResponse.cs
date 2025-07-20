using System.Globalization;
using Newtonsoft.Json;

namespace HCM.Features.Identity.Contracts;

public sealed class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    [JsonIgnore] public DateTime AccessExpiry { get; set; }
    [JsonIgnore] public DateTime RefreshExpiry { get; set; }
    
    public string AccessTokenExpiry => AccessExpiry.ToLocalTime().ToString(CultureInfo.InvariantCulture);

    public int RefreshTokenValidityMinutes => (int)RefreshExpiry.Subtract(DateTime.UtcNow).TotalMinutes;
}
