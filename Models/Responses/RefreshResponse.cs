namespace Models;

public record RefreshResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
