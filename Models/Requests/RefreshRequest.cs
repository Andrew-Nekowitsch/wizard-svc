namespace Models.Requests;

public record RefreshRequest
{
    public required string Token { get; set; }
}