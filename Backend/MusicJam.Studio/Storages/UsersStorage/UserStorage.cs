using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrastructure.Data;
using DbUser = MusicJam.Studio.Infrastructure.Entities.User;
using User = MusicJam.Studio.Models.Users.User;

namespace MusicJam.Studio.Storages.UsersStorage;

internal class UserStorage(AppDbContext db) : IUserStorage
{
    public async Task<User?> GetUserByEmail(string email)
    {
        var entity =  await db.Users
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();
        return entity is null ? null : ToModel(entity);
    }

    public async Task<User> SaveUser(User user)
    {
        var saved = new DbUser()
        {
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.Password
        };
        await db.Users.AddAsync(saved);
        await db.SaveChangesAsync();
        return ToModel(saved);
    }
    
    private static User ToModel(DbUser e) => new(e.Email, e.PasswordHash, e.Username)
    {
        Id = e.Id
    };

}
