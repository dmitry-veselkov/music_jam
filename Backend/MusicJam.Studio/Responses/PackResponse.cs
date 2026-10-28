using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Responses;

public class GetPackResponse
{
    [Required]
    public string PackId { get; set; } 
    public string Name { get; set; } 
    public string CreatedAtUtc { get; set; }
    public string UpdatedAtUtc { get; set; }
}

public class EditPackResponse
{
    [Required]
    public Guid PackId { get; set; }
    
    public string Name { get; set; }
    
    public List<RoundResponse> Rounds { get; set; }
}