namespace MusicJam.Domain.Models.Auth;

public sealed record AuthCredentials(
    string Email,
    string Password
);