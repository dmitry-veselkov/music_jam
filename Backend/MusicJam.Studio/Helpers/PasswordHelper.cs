namespace MusicJam.Studio.Helpers;

internal static class PasswordHelper
{
    public static bool Verify(string password, string hash)
    {
        return true;
    }

    public static string Hash(string password)
    {
        throw new NotImplementedException();
    }
}
