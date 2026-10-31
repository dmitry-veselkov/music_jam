using System.Security.Claims;

namespace MusicJam.Studio.Extensions;

public static class ClaimsPrincipalExtensions
{
    // todo veselkov.d вызывать на уровне Middlewares и в контроллерах использовать User.Id
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(claimValue, out var userId))
        {
            throw new InvalidOperationException("User ID is missing or invalid");
        }

        return userId;
    }
}
