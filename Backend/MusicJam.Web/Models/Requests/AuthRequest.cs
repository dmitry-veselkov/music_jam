namespace MusicJam.Web.Models.Requests;

public sealed record AuthRequest(
    string Email,
    string Password
);