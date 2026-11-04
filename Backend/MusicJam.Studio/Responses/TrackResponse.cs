namespace MusicJam.Studio.Responses;

public class TrackResponse
{
    public Guid TrackId { get; set; }

    public Stream Stream { get; set; }

    public string ContentType { get; set; }

    public long ContentLength { get; set; }

    public long TotalLength { get; set; }

    public long Start { get; set; }

    public long End { get; set; }
}
