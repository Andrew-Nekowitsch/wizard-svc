
using Data.Commands;
using Data.Queries;
using Models;
using Utilities;

namespace Services;

public interface IAccountService
{
    Task<MessageWrapper<GetAccountResponse>> CreateUserAsync(string email, string username, string password);
    Task<MessageWrapper<GetAccountResponse>> ValidateUserAsync(string login, string password);
    Task<MessageWrapper<GetAccountResponse>> GetUserByIdAsync(string userId);
}

public class AccountService(IAccountCommands accountCommands, IAccountQueries accountQueries) : IAccountService
{
    public async Task<MessageWrapper<GetAccountResponse>> CreateUserAsync(string email, string username, string passwordRaw)
    {
        try
        {
            var passwordHashed = AccountUtilities.HashPassword(passwordRaw);
            var accountWrapper = await accountCommands.Register(email, username, passwordHashed);

            if (accountWrapper == null || accountWrapper.Data == null || !accountWrapper.Success)
            {
                return new MessageWrapper<GetAccountResponse>(accountWrapper?.Message ?? "Unknown error occurred.", accountWrapper?.Error ?? [], false, null);
            }

            var safeAccount = accountWrapper.Data?.ToGetAccountResponse();

            return new MessageWrapper<GetAccountResponse>("Registration successful.", [], true, safeAccount);
        }
        catch (Exception ex)
        {
            return new MessageWrapper<GetAccountResponse>(ex.Message, [new ErrorMessage("exception", ex.Message)], false, null);
        }
    }

    public async Task<MessageWrapper<GetAccountResponse>> ValidateUserAsync(string login, string passwordRaw)
    {
        try
        {
            var lowerLogin = login.ToLower().Trim();
            var msg = await accountQueries.GetAccount(lowerLogin);

            if (!msg.Success)
            {
                return new MessageWrapper<GetAccountResponse>(msg.Message, [new ErrorMessage("login", msg.Message)], false, null);
            }

            var user = msg.Data;
            if (user != null && !AccountUtilities.VerifyPassword(passwordRaw, user.PasswordHash))
            {
                return new MessageWrapper<GetAccountResponse>("Invalid password.", [new ErrorMessage("password", "Invalid password.")], false, null);
            }

            GetAccountResponse? safeUser = null;
            if (user != null)
            {
                safeUser = new GetAccountResponse
                {
                    Id = user.Id,
                    Username = user.UserName
                };
            }

            return new MessageWrapper<GetAccountResponse>("Sign-in successful.", [], true, safeUser);
        }
        catch (Exception ex)
        {
            return new MessageWrapper<GetAccountResponse>(ex.Message, [new ErrorMessage("exception", ex.Message)], false, null);
        }
    }

    public async Task<MessageWrapper<GetAccountResponse>> GetUserByIdAsync(string userId)
    {
        try
        {
            var msg = await accountQueries.GetAccountById(userId);

            if (!msg.Success || msg.Data == null)
            {
                return new MessageWrapper<GetAccountResponse>("User not found.", [new ErrorMessage("id", "User id not found.")], false, null);
            }

            var user = msg.Data;
            var safeUser = new GetAccountResponse
            {
                Id = user.Id,
                Username = user.UserName
            };

            return new MessageWrapper<GetAccountResponse>("User retrieved successfully.", [], true, safeUser);
        }
        catch (Exception ex)
        {
            return new MessageWrapper<GetAccountResponse>(ex.Message, [new ErrorMessage("exception", ex.Message)], false, null);
        }
    }
}