using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data.Commands;
using Data.Queries;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Services;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string username);
    string GenerateRefreshToken();
    Task SaveAsync(string userId, string refreshToken, DateTime expires);
    Task<MessageWrapper<RefreshToken>> GetAsync(string token);
}

public class TokenService(IOptions<JwtSettings> settings, IAuthCommands authCommands, IAuthQueries authQueries) : ITokenService
{
    private readonly JwtSettings _settings = settings.Value;

    public string GenerateAccessToken(string userId, string username)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.Secret);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public string GenerateRefreshToken()
    {
        try
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public Task SaveAsync(string userId, string refreshToken, DateTime expires)
    {
        authCommands.SaveTokenAsync(userId, refreshToken, expires);
        return Task.CompletedTask;
    }

    public async Task<MessageWrapper<RefreshToken>> GetAsync(string token)
    {
        // This method should retrieve the refresh token from the database
        // For now, we return a placeholder message
        return await authQueries.GetTokenAsync(new RefreshToken { Token = "token" }); // Replace with actual userId retrieval logic
    }
}
