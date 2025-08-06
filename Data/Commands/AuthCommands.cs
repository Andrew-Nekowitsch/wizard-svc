
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Commands;

public interface IAuthCommands
{
    Task<MessageWrapper<string>> SaveTokenAsync(RefreshToken refreshToken);
    Task RevokeTokenAsync(string token);
}

public class AuthCommands(AppDbContext context) : IAuthCommands
{
    private readonly AppDbContext context = context;

    public async Task<MessageWrapper<string>> SaveTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();
            
            return new MessageWrapper<string>("Token saved.", [], false, string.Empty);
        }
        catch (Exception ex)
        {
            return new MessageWrapper<string>(ex.Message, [], false, "Failed to save token.");
        }
    }

    public async Task RevokeTokenAsync(string token)
    {
        var storedToken = await context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == token);

        if (storedToken == null) return;

        context.RefreshTokens.Remove(storedToken);

        await context.SaveChangesAsync();
    }
}