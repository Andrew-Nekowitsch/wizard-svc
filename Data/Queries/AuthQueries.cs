using Microsoft.EntityFrameworkCore;
using Models;
using Utilities;

namespace Data.Queries;

public interface IAuthQueries
{
    Task<MessageWrapper<RefreshToken>> GetTokenAsync(RefreshToken refreshToken);
}

public class AuthQueries(AppDbContext context) : IAuthQueries
{
    private readonly AppDbContext context = context;

    public async Task<MessageWrapper<RefreshToken>> GetTokenAsync(RefreshToken refreshToken)
    {
        // var token = await context.RefreshTokens
        //     .Where(t => t.UserId == refreshToken.UserId)
        //     .Select(t => t.Token)
        //     .FirstOrDefaultAsync();

        // if (token == null)
        // {
        //     return new MessageWrapper<string>("Token not found.", false, null);
        // }
        await Task.Delay(100); // Simulate async operation

        return new MessageWrapper<RefreshToken>("Token retrieved successfully.", [], true, new RefreshToken { UserId = "userId", Token = "token" });
    }
}