namespace MusicJam.Studio.Models.Requests;

public class PacksWithPaginationRequest
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
}
