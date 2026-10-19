using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations.Services;

public class UserHandler
{
    private readonly AppDbContext _dataContext;
    
    public UserHandler(AppDbContext dataContext)
    {
        _dataContext = dataContext;
    }
}