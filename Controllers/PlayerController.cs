using Microsoft.AspNetCore.Mvc;
using Data.Commands;
using Data.Queries;
using Models;
using Models.Requests;
using Models.Responses;
using Utilities;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController(IPlayerCommands playerCommands, IPlayerQueries playerQueries, IAccountQueries accountQueries) : ControllerBase
{
    private readonly IPlayerCommands _playerCommands = playerCommands;
    private readonly IPlayerQueries _playerQueries = playerQueries;
    private readonly IAccountQueries _accountQueries = accountQueries;


    [HttpGet("GetPlayers")]
    public async Task<IActionResult> GetPlayers(int page = 1, int pageSize = 10)
    {
        var players = await _playerQueries.GetPlayers(page, pageSize);
        return Ok(players.Data);
    }
}