using MusicJam.Runtime.Constants;

namespace MusicJam.Runtime.Middlewares;

public sealed class SessionMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        if (!Guid.TryParse(context.Request.Cookies[SessionCookie.Name], out var sessionId))
        {
            sessionId = Guid.NewGuid();
            context.Response.Cookies.Append(SessionCookie.Name, sessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/runtime",
                MaxAge = TimeSpan.FromHours(12),
            });
        }
        context.Items[SessionCookie.Name] = sessionId;
        return next(context);
    }
}

public static class SessionHttpContextExtensions
{
    public static Guid GetSessionId(this HttpContext context) =>
        context.Items[SessionCookie.Name] is Guid sessionId
            ? sessionId
            : throw new InvalidOperationException("SessionMiddleware не подключена");
}
