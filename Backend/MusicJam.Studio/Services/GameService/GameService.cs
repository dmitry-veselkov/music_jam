using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Storages;
using Shared;

namespace MusicJam.Studio.Services.GameService;

public class GameService(
    IGameStorage gameStorage)
    : IGameService
{
    public Task<Result<List<Game>, IGameService.GameError>> GetQuestions(Guid userId, GetGameRequests query)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Game, IGameService.GameError>> Create(Guid userId, CreateGameRequests query)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Game, IGameService.GameError>> Get(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Guid, IGameService.GameError>> Delete(Guid userId, Guid gameId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Game, IGameService.GameError>> Update(Guid userId, Guid gameId, UpdateGameRequests request)
    {
        throw new NotImplementedException();
    }
}