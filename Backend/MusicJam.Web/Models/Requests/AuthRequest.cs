namespace MusicJam.Web.Models.Requests;

public sealed record AuthRequest(
    string? Name,
    string Email,
    string Password
);