using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations.Services;

public class GameHandler
{
    private readonly AppDbContext _dataContext;
    
    public GameHandler(AppDbContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Game Build(Guid packId)
    {
        throw new NotImplementedException();
    } 
}