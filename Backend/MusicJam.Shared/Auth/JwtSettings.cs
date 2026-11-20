namespace Shared.Auth;

public class JwtSettings
{
    public const string JwtSettingsSectionName = "JwtSettings";

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
}
