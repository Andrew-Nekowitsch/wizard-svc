
using Data.Commands;
using Data.Queries;
using Models.Responses;
using Models.Util;

namespace Services;

public interface ISpellService
{
    Task<GetSpellsResponse> GetSpellsByPlayerIdAsync(int playerId);
}

public class SpellService(ISpellCommands spellCommands, ISpellQueries spellQueries) : ISpellService
{
    public async Task<GetSpellsResponse> GetSpellsByPlayerIdAsync(int playerId)
    {
        try
        {
            var spells = await spellQueries.GetSpellsByPlayerIdAsync(playerId);
            if (spells == null || spells.Count == 0)
            {
                return new GetSpellsResponse(playerId, null)
                {
                    Message = "No spells found.",
                    Success = false,
                    Errors = [],
                    Spells = null
                };
            }

            return new GetSpellsResponse(playerId, spells)
            {
                Message = "Spells retrieved successfully.",
                Errors = [],
                Success = true,
                Spells = spells
            };
        }
        catch (Exception ex)
        {
            return new GetSpellsResponse(playerId, null)
            {
                Message = ex.Message,
                Errors = [new ErrorMessage("exception", ex.Message)],
                Success = false,
                Spells = null
            };
        }
    }
}