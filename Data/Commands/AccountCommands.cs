
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

        var errors = new List<ErrorMessage>();

        if (await context.Accounts.AnyAsync(u => u.Email == email))
        {
            errors.Add(new ErrorMessage("Email already exists.", "email"));
        }
        
        var normalizedUsername = username.ToLower().Trim();
        if (await context.Accounts.AnyAsync(u => u.UserName.ToLower().Trim() == normalizedUsername))
        {
            errors.Add(new ErrorMessage("Username already exists.", "username"));
        }

        if (errors.Count != 0)
        {
            return new MessageWrapper<Account>("Validation failed.", errors, false, null);
        }

        await context.Accounts.AddAsync(user);
        await context.SaveChangesAsync();
        return new MessageWrapper<Account>("User registered successfully.", new List<ErrorMessage>(), true, user);
    }
}