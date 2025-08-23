using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data.Commands;
using Data.Queries;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Data;
using Models.Util;

namespace Services;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string username);
    string GenerateRefreshToken();
    Task<MessageWrapper<string>> SaveAsync(RefreshToken refreshToken);
    Task<MessageWrapper<RefreshToken>> GetAsync(string token);
    Task<MessageWrapper<string>> RevokeAsync(string token);
}

public class TokenService(IOptions<JwtSettings> settings, IAuthCommands authCommands, IAuthQueries authQueries) : ITokenService
{
    private readonly JwtSettings _settings = settings.Value;

    public string GenerateAccessToken(string userId, string username)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes).ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception)
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

    public async Task<MessageWrapper<string>> SaveAsync(RefreshToken refreshToken)
    {
        try
        {
            await authCommands.SaveTokenAsync(refreshToken);
            return new MessageWrapper<string>("Refresh token saved successfully.", [], true, null);
        }
        catch (Exception ex)
        {
            return new MessageWrapper<string>("Failed to save refresh token.", [new ErrorMessage("token", ex.Message)], false, null);
        }
    }

    public async Task<MessageWrapper<RefreshToken>> GetAsync(string token)
    {
        try
        {
            var result = await authQueries.GetTokenAsync(token);
            if (!result.Success)
            {
                return new MessageWrapper<RefreshToken>("Failed to retrieve token.", result.Error, false, null);
            }

            return new MessageWrapper<RefreshToken>("Token retrieved successfully.", [], true, result.Data);
        }
        catch (Exception ex)
        {
            return new MessageWrapper<RefreshToken>("Exception thrown while retrieving token.", [new ErrorMessage("token", ex.Message)], false, null);
        }
    }

    public async Task<MessageWrapper<string>> RevokeAsync(string token)
    {
        try
        {
            await authCommands.RevokeTokenAsync(token);
            return new MessageWrapper<string>("Token revoked successfully.", [], true, null);
        }
        catch (Exception ex)
        {
            return new MessageWrapper<string>("Exception thrown while revoking token.", [new ErrorMessage("token", ex.Message)], false, null);
        }
    }
}
