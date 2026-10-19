using MusicJam.Domain.Entities;

namespace MusicJam.Infrastructure.Data.Configurations.Services;

public class PackHandler
{
    private readonly AppDbContext _dataContext;
    
    public PackHandler(AppDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<Pack?> Build(Guid packId)
        {
            throw new NotImplementedException();
        } 
}
