using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.GameStorage;

public interface IGameStorage
{
    Task<bool> Add(Game game);
    Task<Game?> Get(Guid gameId);
}
