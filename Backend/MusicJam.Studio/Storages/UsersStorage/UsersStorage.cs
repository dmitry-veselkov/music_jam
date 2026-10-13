using MusicJam.Studio.Entities;

namespace MusicJam.Studio.Storages.UsersStorage;

internal class UsersStorage : IUsersStorage
{
    public async Task<User?> TryGetUserByEmail(string email)
    {
        await Task.CompletedTask;

        return new User(email, "some_test_password_hash_123", "James Bond");
    }

    public async Task<User> SaveUser(User user)
    {
        await Task.CompletedTask;

        return new User(user.Email, user.PasswordHash, user.Name);
    }
}
