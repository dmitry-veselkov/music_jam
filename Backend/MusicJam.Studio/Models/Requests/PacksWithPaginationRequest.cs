using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Models.Requests;

public class PacksWithPaginationRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "PageNumber must be greater than or equal to 0")]
    public int Skip { get; set; } = 0;

    [Range(1, 20, ErrorMessage = "PageSize must be greater than or equal to 20")]
    public int Take { get; set; } = 10;
}
