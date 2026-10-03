using MusicJam.Domain.Models;

namespace MusicJam.Domain.Storages;

public interface IUsersStorage
{
    Task<User?> TryGetUserByEmail(string email);
}