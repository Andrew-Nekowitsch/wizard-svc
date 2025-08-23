using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Requests;
using Models;
using Microsoft.Extensions.Options;
using Models.Responses;
using Microsoft.AspNetCore.Authorization;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpellController(ISpellService spellService) : ControllerBase
{
    [HttpGet("spells")]
    public async Task<IActionResult> GetSpells(int playerId)
    {
        var spells = await spellService.GetSpellsByPlayerIdAsync(playerId);
        return Ok(spells);
    }
}