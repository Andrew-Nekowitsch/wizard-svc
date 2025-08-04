
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Commands;

public interface IAuthCommands
{
    Task<MessageWrapper<string>> SaveTokenAsync(string id, string refreshToken, DateTime expires);
}

public class AuthCommands(AppDbContext context) : IAuthCommands
{
    private readonly AppDbContext context = context;

    public async Task<MessageWrapper<string>> SaveTokenAsync(string id, string refreshToken, DateTime expires)
    {
        await Task.CompletedTask; // Simulating async operation, replace with actual implementation
        return new MessageWrapper<string>("Method not implemented.", [], false, null);
        // try
        // {
        //     // context.RefreshTokens.Add(new RefreshToken
        //     // {
        //     //     UserId = id,
        //     //     Token = refreshToken,
        //     //     Expires = expires
        //     // });
        // }
        // catch (Exception ex)
        // {
        //     return new MessageWrapper<void>(ex.Message, false, null);
        // }
    }
}