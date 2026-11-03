using CS2Storage.Data;
using CS2Storage.DTOs;
using CS2Storage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CS2Storage.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BotsController : ControllerBase{
    private readonly StorageDbContext _context;
    public BotsController(StorageDbContext context){
        _context = context;
    }

    [HttpGet]//get todos
    public async Task<IActionResult> GetBots(){
        var bots = await _context.Bots.AsNoTracking().Select(b => new BotResponse(b.Id,b.SteamId,b.Name,b.IsActive)).ToListAsync();
        return Ok(bots);
    }

    [HttpGet("{id:guid}")]//get por id
    public async Task<IActionResult> GetBotById(Guid id){
        var bot = await _context.Bots.AsNoTracking().Where(b => b.Id == id).Select(b => new BotResponse(b.Id,b.SteamId,b.Name,b.IsActive)).FirstOrDefaultAsync();
        if (bot == null) return NotFound("Bot não encontrado.");
        return Ok(bot);
    }

    [HttpPost]//cria bot
    public async Task<IActionResult> CreateBot([FromBody] CreateBotRequest request){
        var alreadyExists = await _context.Bots.AnyAsync(b => b.SteamId == request.SteamId);
        if (alreadyExists) return Conflict("Essa conta Steam já está cadastrada.");
        var bot = new Bot{
            SteamId = request.SteamId,
            Name = request.Name.Trim(),
            IsActive = true
        };

        _context.Bots.Add(bot);
        await _context.SaveChangesAsync();
        var response = new BotResponse(bot.Id,bot.SteamId,bot.Name,bot.IsActive);
        return CreatedAtAction(nameof(GetBotById),new { id = bot.Id },response);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateBotStatus(Guid id,[FromBody] UpdateBotStatusRequest request){
        var bot = await _context.Bots.FindAsync(id);
        if (bot == null) return NotFound("Bot não encontrado.");
        if (request.IsActive == null) return BadRequest("O campo IsActive é obrigatório.");
        bot.IsActive = request.IsActive.Value;
        await _context.SaveChangesAsync();
        return Ok(new BotResponse(bot.Id, bot.SteamId,bot.Name,bot.IsActive));
    }
}