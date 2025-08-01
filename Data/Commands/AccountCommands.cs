
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Commands;

public interface IAccountCommands
{
    Task<MessageWrapper<Account>> Register(string email, string username, string passwordHashed);
}

public class AccountCommands(AppDbContext context) : IAccountCommands
{
    private readonly AppDbContext context = context;

    public async Task<MessageWrapper<Account>> Register(string email, string username, string passwordHashed)
    {
        var user = new Account
        {
            Email = email.ToLower().Trim(),
            UserName = username.ToLower().Trim(),
            DisplayName = username,
            PasswordHash = passwordHashed
        };

        if (await context.Accounts.AnyAsync(u => u.Email == email))
        {
            return new MessageWrapper<Account>("Email already exists.", false, null);
        }
        
        var normalizedUsername = username.ToLower().Trim();
        if (await context.Accounts.AnyAsync(u => u.UserName.ToLower().Trim() == normalizedUsername))
        {
            return new MessageWrapper<Account>("Username already exists.", false, null);
        }

        await context.Accounts.AddAsync(user);
        await context.SaveChangesAsync();
        return new MessageWrapper<Account>("User registered successfully.", true, user);
    }
}