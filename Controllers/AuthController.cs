using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Requests;
using Models;
using Microsoft.Extensions.Options;
using Models.Responses;
using Microsoft.AspNetCore.Authorization;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAccountService accountService, ITokenService tokenService, IOptions<JwtSettings> jwtSettings) : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    private bool useSecure = false;

    [HttpPost("register")]
    public async Task<ActionResult<MessageWrapper<LoginResponse>>> Register([FromBody] SignUpRequest request)
    {
        var accountResponseWrapper = await accountService.CreateUserAsync(request);

        if (accountResponseWrapper == null || accountResponseWrapper.Validate().ContainsErrors())
        {
            return BadRequest(accountResponseWrapper);
        }

        return Ok(new MessageWrapper<string>("Registration successful", [], true, string.Empty));
    }

    [HttpPost("login")]
    public async Task<ActionResult<MessageWrapper<LoginResponse>>> Login([FromBody] SignInRequest request)
    {
        var accountMsg = await accountService.ValidateUserAsync(request);
        if (accountMsg == null || accountMsg.Validate().ContainsErrors())
        {
            return Unauthorized(new MessageWrapper<LoginResponse>("Invalid login or password.", [new ErrorMessage("login", "Invalid login or password.")], false, null));
        }

        var accessToken = tokenService.GenerateAccessToken(accountMsg.Data!.Id.ToString(), request.Login);
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized(new MessageWrapper<LoginResponse>("Failed to generate access token.", [new ErrorMessage("accessToken", "Failed to generate access token.")], false, null));
        }

        var newRefreshToken = tokenService.GenerateRefreshToken();
        if (string.IsNullOrEmpty(newRefreshToken))
        {
            return Unauthorized(new MessageWrapper<LoginResponse>("Failed to generate refresh token.", [new ErrorMessage("refreshToken", "Failed to generate refresh token.")], false, null));
        }
        
        var savedRefreshToken = new RefreshToken()
        {
            UserId = accountMsg.Data.Id.ToString(),
            Token = newRefreshToken,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await tokenService.SaveAsync(savedRefreshToken);

        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,                    // 🛡️ Prevents JS access
            Secure = useSecure,                     // 🔒 Only sent over HTTPS (true in production)
            SameSite = SameSiteMode.Strict,     // 🚧 Prevents cross-site CSRF
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        });

        return Ok(new MessageWrapper<LoginResponse>("Login successful", [], true, new LoginResponse
        {
            AccessToken = accessToken,
            UserWrapper = accountMsg
        }));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken != null)
        {
            await tokenService.RevokeAsync(refreshToken);
        }

        Response.Cookies.Append("refreshToken", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = useSecure,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });

        return Ok(new { message = "Logged out" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var tokenMsg = await tokenService.GetAsync(refreshToken);
        if (tokenMsg == null || tokenMsg.Validate().ContainsErrors() || tokenMsg.Data!.Revoked != null || tokenMsg.Data!.IsExpired)
            return Unauthorized();

        var refreshTokenDb = tokenMsg.Data!;
        var userMsg = await accountService.GetUserByIdAsync(tokenMsg.Data!.UserId);
        if (userMsg == null || userMsg.Validate().ContainsErrors())
            return Unauthorized();

        var user = userMsg.Data!;
        refreshTokenDb.Revoked = DateTime.UtcNow;
        var newAccessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Username);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        var savedRefreshToken = new RefreshToken()
        {
            UserId = user.Id.ToString(),
            Token = newRefreshToken,
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await tokenService.SaveAsync(savedRefreshToken);

        // Set new refresh cookie
        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = useSecure, // false in dev if you're using HTTP
            SameSite = SameSiteMode.Strict,
            Expires = savedRefreshToken.Expires
        });

        return Ok(new { accessToken = newAccessToken });
    }

    [Authorize]
    [HttpGet("authenticated")]
    public async Task<IActionResult> Authenticated()
    {
        await Task.CompletedTask;
        return Ok("Authenticated");
    }
}