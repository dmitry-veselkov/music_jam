namespace MusicJam.Runtime.Models;

public class GameInvite
{
    /// <summary>
    /// Id приглашения
    /// </summary>
    public Guid InviteId { get; set; }

    /// <summary>
    /// Id игры, для которой создано приглашение
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// N-значный код для подключения к игре с главного экрана
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Ссылка для подключения к игре
    /// </summary>
    public string Link { get; set; }
}