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
public class AuthController(IAccountService accountService, ITokenService tokenService, IOptions<JwtSettings> jwtSettings, IConfiguration configuration) : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    [HttpPost("register")]
    public async Task<ActionResult<MessageWrapper<LoginResponse>>> Register([FromBody] SignUpRequest request)
    {
        var accountResponseWrapper = await accountService.CreateUserAsync(request);

        if (accountResponseWrapper == null || accountResponseWrapper.Validate().ContainsErrors())
        {
            return BadRequest(accountResponseWrapper);
        }
        
        var tokens = await tokenService.GenerateTokens(accountResponseWrapper.Data!.Id.ToString(), accountResponseWrapper.Data.Username, _jwtSettings.AccessTokenExpirationMinutes);

        return Ok(new MessageWrapper<LoginResponse>("Registration successful", [], true, new LoginResponse
        {
            Tokens = tokens,
            UserWrapper = accountResponseWrapper
        }));
    }

    [HttpPost("login")]
    public async Task<ActionResult<MessageWrapper<LoginResponse>>> Login([FromBody] SignInRequest request)
    {
        var accountResponseWrapper = await accountService.ValidateUserAsync(request);
        if (accountResponseWrapper == null || accountResponseWrapper.Validate().ContainsErrors())
        {
            return Unauthorized(new MessageWrapper<LoginResponse>("Invalid login or password.", [new ErrorMessage("login", "Invalid login or password.")], false, null));
        }

        var tokens = await tokenService.GenerateTokens(accountResponseWrapper.Data!.Id.ToString(), accountResponseWrapper.Data.Username, _jwtSettings.AccessTokenExpirationMinutes);

        return Ok(new MessageWrapper<LoginResponse>("Login successful", [], true, new LoginResponse
        {
            Tokens = tokens,
            UserWrapper = accountResponseWrapper
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

    return Ok(new { message = "Logged out" });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        await Task.CompletedTask;
        return Ok();
    }

    [Authorize]
    [HttpPost("authenticated")]
    public async Task<IActionResult> Authenticated()
    {
        await Task.CompletedTask;
        return Ok("Authenticated");
    }
}