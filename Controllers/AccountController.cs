using Microsoft.AspNetCore.Mvc;
using Data.Queries;
using Services;
using Models.Requests;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SignUpRequest request)
    {
        var result = await _accountService.CreateUserAsync(request.Email, request.Username, request.Password);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}