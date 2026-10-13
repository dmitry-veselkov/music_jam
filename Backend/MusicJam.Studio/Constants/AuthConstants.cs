namespace MusicJam.Studio.Constants;

public abstract class AuthConstants
{
    public const string DefaultAuthorizationScheme = "AccountJwtTokenScheme";
    public const string AccessToken = "X-Access-Token";
    public const int AccessTokenLifetimeInDays = 3;
}
