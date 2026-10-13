using MusicJam.Studio.Entities;

namespace MusicJam.Studio.Storages.UsersStorage;

public interface IUsersStorage
{
    Task<User?> TryGetUserByEmail(string email);

    Task<User> SaveUser(User user);
}
