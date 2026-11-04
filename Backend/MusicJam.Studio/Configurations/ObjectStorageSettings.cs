namespace MusicJam.Studio.Configurations;

public class ObjectStorageSettings
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public int MaxFileSizeInBytes { get; set; } = 30 * 1024 * 1024; // 30 Mb
    public string ObjectStorageBaseUrl { get; set; } = "https://storage.yandexcloud.net";
}
