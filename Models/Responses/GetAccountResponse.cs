namespace Models.Responses;

public record GetAccountResponse
{
    public int Id { get; set; }
    public required string Username { get; set; }
}