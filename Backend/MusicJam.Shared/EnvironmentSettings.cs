namespace Shared;

public class EnvironmentSettings
{
    public const string SectionName = "Environment";

    public bool SslEnabled { get; set; }
    public string Domain { get; set; }
    public int? Port { get; set; }

    private string Protocol => SslEnabled ? "https" : "http";
    private string PortSuffix => Port.HasValue ? $":{Port.Value}" : string.Empty;

    public string BaseUrl => $"{Protocol}://{Domain}{PortSuffix}";
}