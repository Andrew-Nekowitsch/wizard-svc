using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await Task.CompletedTask;
        return Ok("Wizard Idler is running.");
    }

    [HttpGet("config")]
    public async Task<IActionResult> GetConfig()
    {
        await Task.CompletedTask;
        return Ok(new
        {
            UI_URL = _configuration["UI_URL"],
            DB_SERVER = _configuration["DB_SERVER"],
            DB_USER = _configuration["DB_USER"],
        });
    }

}