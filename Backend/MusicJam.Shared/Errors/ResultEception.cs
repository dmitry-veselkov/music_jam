namespace Shared.Errors;

public class ResultException<TError>(TError error) : Exception
{
    public TError Error { get; } = error;
}
