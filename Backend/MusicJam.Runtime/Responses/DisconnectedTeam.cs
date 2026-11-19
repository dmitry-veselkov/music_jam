using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Responses;

public sealed record DisconnectedTeam(Guid GameId, Team Team);
