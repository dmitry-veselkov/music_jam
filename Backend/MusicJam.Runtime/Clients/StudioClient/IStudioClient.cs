using MusicJam.Studio.Models.Packs;

namespace MusicJam.Runtime.Clients.StudioClient;

public interface IStudioClient
{
    Task<Pack?> GetPack(Guid packId, CancellationToken cancellationToken = default);
}
