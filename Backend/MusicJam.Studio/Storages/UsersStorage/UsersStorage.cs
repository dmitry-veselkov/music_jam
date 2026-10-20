using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbUser = MusicJam.Studio.Infrustructure.Entities.User;
using User = MusicJam.Studio.Models.Users.User;

namespace MusicJam.Studio.Storages.UsersStorage;

internal class UsersStorage(AppDbContext db) : IUsersStorage
{
    public async Task<User?> TryGetUserByEmail(string email)
    {
        var entity =  await db.Users
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync();
        return entity is null ? null : ToModel(entity);
    }

    public async Task<User> SaveUser(User user)
    {

        await db.Users.AddAsync(new DbUser()
        {
            Username = user.Username,
            Email = user.Email,
            Password = user.Password
        });
        await db.SaveChangesAsync();
        return new User(user.Email, user.Password, user.Username);
    }
    
    private static User ToModel(DbUser e) => new(e.Email, e.Password, e.Username);

}
