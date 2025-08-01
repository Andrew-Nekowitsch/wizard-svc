namespace Models.Requests;

public record SignUpRequest
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}