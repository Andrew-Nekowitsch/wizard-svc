using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Queries;

public interface IPlayerQueries
{
    Task<MessageWrapper<Player?>> GetPlayer(int playerId);
    Task<MessageWrapper<List<Player>?>> GetPlayers(int page, int pageSize);
}

public class PlayerQueries : IPlayerQueries
{
    private readonly AppDbContext context;

    public PlayerQueries(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<MessageWrapper<Player?>> GetPlayer(int playerId)
    {
        MessageWrapper<Player?>? response = new MessageWrapper<Player?>( "Player could not be found.", false, default!);
        try
        {
            var player = await context.Players
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player != null)
            {
                response = new MessageWrapper<Player?>("Player found successfully.", true, player);
            }
        }
        catch (Exception ex)
        {
            response = new MessageWrapper<Player?>(ex.Message, false, default!);
        }
        return response;
    }

    public async Task<MessageWrapper<List<Player>?>> GetPlayers(int page, int pageSize)
    {
        List<Player>? players = [];
        MessageWrapper<List<Player>?> response = new MessageWrapper<List<Player>?>("Players could not be found.", false, players);
        try
        {
            players = await context.Players
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            response = new MessageWrapper<List<Player>?>("Players found successfully.", true, players);
        }
        catch (Exception ex)
        {
            response = new MessageWrapper<List<Player>?>(ex.Message, false, players);
        }
        return response;
    }
}