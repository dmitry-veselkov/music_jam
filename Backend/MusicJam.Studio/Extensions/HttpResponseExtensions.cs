using MusicJam.Studio.Constants;

namespace MusicJam.Studio.Extensions;

public static class HttpResponseExtensions
{
    public static void SetAccessTokenCookie(this HttpResponse response, string token)
    {
        response.Cookies.Append(AuthConstants.AccessToken, token, GetTokenCookieOptions());
    }

    private static CookieOptions GetTokenCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(AuthConstants.AccessTokenLifetimeInDays)
    };
}
