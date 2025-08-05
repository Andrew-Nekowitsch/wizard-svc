
using Data.Commands;
using Data.Queries;
using Models;
using Models.Requests;
using Models.Responses;
using Utilities;

namespace Services;

public interface IAccountService
{
    Task<MessageWrapper<GetAccountResponse>> CreateUserAsync(SignUpRequest signUpRequest);
    Task<MessageWrapper<GetAccountResponse>> ValidateUserAsync(SignInRequest signInRequest);
    Task<MessageWrapper<GetAccountResponse>> GetUserByIdAsync(string userId);
}

public class AccountService(IAccountCommands accountCommands, IAccountQueries accountQueries) : IAccountService
{
    public async Task<MessageWrapper<GetAccountResponse>> CreateUserAsync(SignUpRequest signUpRequest)
    {
        try
        {
            var passwordHashed = AuthUtilities.HashPassword(signUpRequest);
            var accountWrapper = await accountCommands.Register(signUpRequest.Email, signUpRequest.Username, passwordHashed);

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

    public async Task<MessageWrapper<GetAccountResponse>> ValidateUserAsync(SignInRequest signInRequest)
    {
        try
        {
            var lowerLogin = signInRequest.Login.ToLower().Trim();
            var msg = await accountQueries.GetAccount(lowerLogin);

            if (!msg.Success || msg.Data == null)
            {
                return new MessageWrapper<GetAccountResponse>("Invalid login or password.", [new ErrorMessage("login", "Invalid login or password.")], false, null);
            }

            var user = msg.Data;
            if (!AuthUtilities.VerifyPassword(signInRequest, user.PasswordHash))
            {
                return new MessageWrapper<GetAccountResponse>("Invalid password.", [new ErrorMessage("password", "Invalid password.")], false, null);
            }

            var safeUser = user.ToGetAccountResponse();
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