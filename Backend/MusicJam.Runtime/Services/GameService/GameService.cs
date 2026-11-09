using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Services.PackService;
using Game = MusicJam.Runtime.Models.Game;

namespace MusicJam.Runtime.Services.GameService;

public class GameService(IPacksService packsService) : IGameService
{
    //сюда потом взаимодействия по раундам
    public async Task<ActionResult<Game>> Create(Guid hostId, Guid packId)
    {
        var pack = await packsService.GetPack(hostId, packId);
        var gamePack = pack.GetValueOrThrow();
        var newGame = new Game()
        {
            GameId = Guid.NewGuid(),
            HostId = hostId,
            Pack = gamePack,
            PackId = packId
        };
        return newGame;
    }
}
