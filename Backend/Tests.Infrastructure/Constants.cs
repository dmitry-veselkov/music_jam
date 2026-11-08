namespace Tests.Infrastructure;

public static class Constants
{
    public static class User
    {
        public const string Username = "test_user";
        public const string Password = "test_password";
    }

    public static class Database
    {
        public const string Image = "postgres:18";
        public const string Name = "musicjam_test";
    }

    public static class ObjectStorage
    {
        public const string Image = "adobe/s3mock:5.2.3";
        public const string BucketName = "test-bucket";
        public const string AccessKey = "test_access_key";
        public const string SecretKey = "test_secret_key";
        public const int Port = 9090;
    }

    public static class Configuration
    {
        public const string JwtSettingsIssuer = "MusicJam.Tests";
        public const string JwtSettingsAudience = "MusicJam.Tests";
        public const string JwtSettingsSecretKey = "TestSecretKey_AtLeast32CharactersLong!";
    };
}
