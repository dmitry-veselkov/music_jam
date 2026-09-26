using MusicJam.Domain.Models;
using MusicJam.Domain.Storages;

namespace MusicJam.Infrastructure.Storages;

internal class UsersStorage : IUsersStorage
{
    public async Task<User?> TryGetUserByEmail(string email)
    {
        await Task.CompletedTask;

        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = "James",
            LastName = "Bond",
            Email = email,
            PasswordHash = "some_test_password_hash_123"
        };
    }
}