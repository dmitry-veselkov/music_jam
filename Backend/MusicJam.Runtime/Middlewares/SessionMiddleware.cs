using MusicJam.Runtime.Constants;
using MusicJam.Runtime.Hubs;

namespace MusicJam.Runtime.Middlewares;

public sealed class SessionMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        if (!Guid.TryParse(context.Request.Cookies[SessionCookie.Name], out var sessionId))
        {
            // Хаб сессии не выдаёт: negotiate/WebSocket идут параллельно с HTTP-запросами
            // и перезаписали бы куку другим id. Сессию выдают только обычные HTTP-эндпоинты.
            if (context.Request.Path.StartsWithSegments(GameHub.Path))
            {
                return next(context);
            }

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
        context.TryGetSessionId()
        ?? throw new InvalidOperationException("SessionMiddleware не подключена");

    public static Guid? TryGetSessionId(this HttpContext context) =>
        context.Items[SessionCookie.Name] is Guid sessionId ? sessionId : null;
}
