using Microsoft.EntityFrameworkCore;
using Models;
using Utilities;

namespace Data.Queries;

public interface IAccountQueries
{
    Task<MessageWrapper<Account>> GetAccount(string login);
    Task<MessageWrapper<Account>> GetAccountById(string userId);
}

public class AccountQueries(AppDbContext context) : IAccountQueries
{
    private readonly AppDbContext context = context;

    public async Task<MessageWrapper<Account>> GetAccount(string login)
    {
        var user = await context.Accounts.FirstOrDefaultAsync(u => u.Email == login || u.UserName == login);
        if (user == null)
        {
            return new MessageWrapper<Account>("User not found.", false, null);
        }

        return new MessageWrapper<Account>("User Found.", true, user);
    }

    public async Task<MessageWrapper<Account>> GetAccountById(string userId)
    {
        var user = await context.Accounts.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
        if (user == null)
        {
            return new MessageWrapper<Account>("User not found.", false, null);
        }

        return new MessageWrapper<Account>("User found.", true, user);
    }
}