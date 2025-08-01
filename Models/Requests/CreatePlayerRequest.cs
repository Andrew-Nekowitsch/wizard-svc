namespace Models.Requests;

public record CreatePlayerRequest
{
    public required Account Account { get; set; }
    public required Player Player { get; set; }
}