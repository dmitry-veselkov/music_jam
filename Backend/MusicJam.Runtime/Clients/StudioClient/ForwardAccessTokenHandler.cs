using MusicJam.Studio.Constants;

namespace MusicJam.Runtime.Clients.StudioClient;

public sealed class ForwardAccessTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.AccessToken];
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Add("Cookie", $"{AuthConstants.AccessToken}={token}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
