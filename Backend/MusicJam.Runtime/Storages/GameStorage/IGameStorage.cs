using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.GameStorage;

public interface IGameStorage
{
    Task<Game> GetOrCreate(Guid gameId);
    Task<Game?> Get(Guid gameId);
}
