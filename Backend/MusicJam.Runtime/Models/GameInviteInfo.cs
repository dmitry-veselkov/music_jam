namespace MusicJam.Runtime.Models;

/// <summary>
/// Способы подключения к игре
/// </summary>
public class GameInviteInfo
{
    /// <summary>
    /// Подключение по n-значному коду
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Подключение по пригласительной ссылке
    /// </summary>
    public string Link { get; set; }
}