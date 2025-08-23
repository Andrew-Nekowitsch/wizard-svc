using Models.Data;
using Models.Util;

namespace Models.Responses;

public record GetSpellsResponse : Dto
{
    public int PlayerId { get; set; }
    public List<Spell>? Spells { get; set; }

    public GetSpellsResponse(int playerId, List<Spell>? spells) : base()
    {
        PlayerId = playerId;
        Spells = spells;
    }

    public GetSpellsResponse() : base() { }
}