using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests;

public class RegisterRequest
{
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
