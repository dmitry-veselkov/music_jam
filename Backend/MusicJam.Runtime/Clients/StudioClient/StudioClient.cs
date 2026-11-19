using System.Net;

namespace MusicJam.Runtime.Clients.StudioClient;

using MusicJam.Studio.Models.Packs;

public sealed class StudioClient(HttpClient httpClient) : IStudioClient
{
    public async Task<Pack?> GetPack(Guid packId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/packs/{packId}", cancellationToken);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Pack>(cancellationToken);
    }
}
