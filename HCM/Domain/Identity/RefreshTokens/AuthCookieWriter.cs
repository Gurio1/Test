namespace HCM.Domain.Identity.RefreshTokens;

public static class AuthCookieWriter
{
    public static void SetRefreshTokenCookie(HttpContext context, RefreshToken token) =>
        context.Response.Cookies.Append("refreshToken", token.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = token.ExpiresAt
        });
}
