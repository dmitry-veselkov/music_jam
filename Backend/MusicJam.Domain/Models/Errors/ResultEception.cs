namespace MusicJam.Domain.Models.Errors;

public class ResultException<TError>(TError error) : Exception
{
    public TError Error { get; } = error;
}