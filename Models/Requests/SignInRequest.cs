namespace Models.Requests;

public record SignInRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}