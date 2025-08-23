using Models.Data;

namespace Models.Responses;

public record CreatePlayerResponse
{
    public Player? Player { get; set; }
}