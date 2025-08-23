using Microsoft.EntityFrameworkCore;
using Models;
using Models.Data;
using Models.Util;

namespace Data.Queries;

public interface IAuthQueries
{
    Task<MessageWrapper<RefreshToken>> GetTokenAsync(string token);
}

public class AuthQueries(AppDbContext context) : IAuthQueries
{
    private readonly AppDbContext context = context;

    public async Task<MessageWrapper<RefreshToken>> GetTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return new MessageWrapper<RefreshToken>("Token cannot be null or empty.", [new ErrorMessage("token", "Token cannot be null or empty.")], false, null);
        }

        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (refreshToken == null)
        {
            return new MessageWrapper<RefreshToken>("Token not found.", [new ErrorMessage("token", "Token not found.")], false, null);
        }

        return new MessageWrapper<RefreshToken>("Token retrieved successfully.", [], true, refreshToken);
    }
}