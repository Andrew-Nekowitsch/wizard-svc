namespace Models.Responses;

public record TokenResponse()
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}