using MusicJam.Studio.Models.Users;

namespace MusicJam.Studio.Storages.UsersStorage;

public interface IUserStorage
{
    Task<User?> GetUserByEmail(string email);

    Task<User> SaveUser(User user);
}
