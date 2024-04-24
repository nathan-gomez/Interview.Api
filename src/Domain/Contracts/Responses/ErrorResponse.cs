namespace Domain.Contracts.Responses;

public struct ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = error;
    }

    public string Error { get; set; }
}