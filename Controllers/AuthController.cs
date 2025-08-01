using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Requests;
using Microsoft.AspNetCore.Authentication;
using Models;
using Microsoft.Extensions.Options;
using Models.Responses;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAccountService accountService, ITokenService tokenService, IOptions<JwtSettings> jwtSettings) : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] SignInRequest request)
    {
        var userWrapper = await accountService.ValidateUserAsync(request.Login, request.Password);
        if (userWrapper == null || !userWrapper.Success || userWrapper.Data == null) return Unauthorized();

        var user = userWrapper.Data;

        var accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Username);
        var refreshToken = tokenService.GenerateRefreshToken();
        await tokenService.SaveAsync(user.Id.ToString(), refreshToken, DateTime.UtcNow.AddDays(7));

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserWrapper = userWrapper
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        return Ok(new { message = "Logged out" });
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var username = User?.Identity?.Name;
        return Ok(new { username });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var tokenWrapper = await tokenService.GetAsync(request.Token);
        if (tokenWrapper.Data == null || !tokenWrapper.Success || tokenWrapper.Data.Expires < DateTime.UtcNow || tokenWrapper.Data.Revoked != null)
        {
            return Unauthorized();
        }

        var userWrapper = await accountService.GetUserByIdAsync(tokenWrapper.Data.UserId);
        if (userWrapper.Data == null || !userWrapper.Success)
        {
            return Unauthorized();
        }

        var token = tokenWrapper.Data;
        var user = userWrapper.Data;
        var newAccessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Username);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        token.Revoked = DateTime.UtcNow;
        await tokenService.SaveAsync(user.Id.ToString(), newRefreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays));

        if (newAccessToken == null || newRefreshToken == null)
        {
            return StatusCode(500, "Failed to generate new tokens.");
        }

        return Ok(new RefreshResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}