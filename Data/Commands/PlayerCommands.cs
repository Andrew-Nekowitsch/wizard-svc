
using Microsoft.EntityFrameworkCore;
using Models;
using Utilities;

namespace Data.Commands;

public interface IPlayerCommands
{
    Task<MessageWrapper<Player?>> CreatePlayer(Player player);
    Task<MessageWrapper<Player?>> UpdatePlayer(Player player);
    Task<MessageWrapper<string?>> DeletePlayer(int playerId);
}

public class PlayerCommands : IPlayerCommands
{
    private readonly AppDbContext context;

    public PlayerCommands(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<MessageWrapper<Player?>> CreatePlayer(Player player)
    {
        MessageWrapper<Player?>? response = new MessageWrapper<Player?>("Player could not be created.", false, default!);
        try
        {
            var result = await context.Players.AddAsync(player);

            await context.SaveChangesAsync();

            response = new MessageWrapper<Player?>("Player created successfully.", true, result.Entity);
        }
        catch (Exception ex)
        {
            response = new MessageWrapper<Player?>(ex.Message, false, default!);
        }

        return response;
    }

    public async Task<MessageWrapper<Player?>> UpdatePlayer(Player player)
    {
        MessageWrapper<Player?>? response = new MessageWrapper<Player?>("No player found.", false, default!);

        try
        {
            var result = await context.Players
                .FirstOrDefaultAsync(p => p.Id == player.Id);

            if (result != null)
            {
                result.Name = player.Name;
                result.Age = player.Age;

                await context.SaveChangesAsync();

                response = new MessageWrapper<Player?>("Player updated successfully.", true, result);
            }
        }
        catch (Exception ex)
        {
            response = new MessageWrapper<Player?>(ex.Message, false, default!);
        }

        return response;
    }

    public async Task<MessageWrapper<string?>> DeletePlayer(int playerId)
    {
        MessageWrapper<string?>? response = new MessageWrapper<string?>("No player found.", false, "0");

        try
        {
            var result = await context.Players
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (result != null)
            {
                context.Players.Remove(result);

                var num = await context.SaveChangesAsync();

                response = new MessageWrapper<string?>($"{num} player(s) deleted.", true, num.ToString());
            }
        }
        catch (Exception ex)
        {
            response = new MessageWrapper<string?>(ex.Message, false, "0");
        }

        return response;
    }
}