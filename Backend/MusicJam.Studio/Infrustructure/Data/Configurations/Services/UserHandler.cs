using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Entities;

namespace MusicJam.Studio.Infrustructure.Data.Configurations.Services;

public class UserHandler
{
    private readonly AppDbContext _dataContext;
    
    public UserHandler(AppDbContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task DeleteUser(User user)
    {
        _dataContext.Users.Remove(user);
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task<Guid> Insert(User user)
    {
        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();
        return user.Id;
    }
    
    public async Task Update(Guid id, string email, string username)
    {
        var userDb = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (userDb is null)
            return;
        userDb.Email = email;
        userDb.Username = username;
        await _dataContext.SaveChangesAsync();
    }
    
    public async Task<List<Game>> GetGames(Guid id)
    {
        var userDb = await _dataContext.Users
            .Include(user => user.Games)
            .FirstOrDefaultAsync(x => x.Id == id);
        return userDb is null ? new List<Game>() : userDb.Games;
    }
    
    public async Task<List<Question>> GetQuestions(Guid id)
    {
        var userDb = await _dataContext.Users
            .Include(user => user.Questions)
            .FirstOrDefaultAsync(x => x.Id == id);
        return userDb is null ? new List<Question>() : userDb.Questions;
    }
}