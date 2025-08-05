using Microsoft.AspNetCore.Identity;
using Models.Requests;

namespace Utilities;

public class AuthUtilities
{
    private static readonly PasswordHasher<SignInRequest> _passwordHasher = new();

    public static string HashPassword(SignUpRequest signUpRequest)
    {
        var user = new SignInRequest
        {
            Login = signUpRequest.Username,
            Password = signUpRequest.Password
        };
        return _passwordHasher.HashPassword(user, user.Password);
    }

    public static bool VerifyPassword(SignInRequest signInRequest, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(signInRequest, passwordHash, signInRequest.Password);
        return result == PasswordVerificationResult.Success;
    }
}