using MusicJam.Studio.Infrustructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations.Services;

public class StatisticsHandler
{
    private readonly AppDbContext _dataContext;
    
    public StatisticsHandler(AppDbContext dataContext)
    {
        _dataContext = dataContext;
    }
}