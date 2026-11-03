using System.Text.Json;
using CS2Storage.Data;
using CS2Storage.DTOs;
using CS2Storage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CS2Storage.Controllers;

[ApiController]
[Route("api/offers")]
public class OffersController : ControllerBase{
    private readonly StorageDbContext _context;
    public OffersController(StorageDbContext context){
        _context = context;
    }
    [HttpPost]
    public async Task<IActionResult> ReceiveOffer( [FromBody] IncomingOfferRequest request){
        if (request.Items.Any(i => i.AppId != 730 || i.ContextId != "2")){
            return BadRequest("A oferta contém itens que não são de CS2.");
        }

        var bot = await _context.Bots.FirstOrDefaultAsync(b => b.SteamId == request.BotSteamId && b.IsActive);
        if (bot == null) return BadRequest("Bot não encontrado ou inativo.");
        var existing = await _context.IncomingOffers.FirstOrDefaultAsync(o => o.OfferId == request.OfferId);
        if (existing != null) return Conflict("Essa oferta já foi registrada.");
        var offer = new IncomingOffer{
            OfferId = request.OfferId,
            SenderSteamId = request.SenderSteamId,
            BotId = bot.Id,
            ItemsJson = JsonSerializer.Serialize(request.Items),
            Status = "Pending",
            ReceivedAt = DateTime.UtcNow
        };

        _context.IncomingOffers.Add(offer);
        await _context.SaveChangesAsync();

        return Ok(new{
            message = "Oferta registrada com sucesso.",
            offer.Id,
            offer.OfferId,
            offer.Status
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetOffers(){
        var offers = await _context.IncomingOffers.AsNoTracking().OrderByDescending(o => o.ReceivedAt).Select(o => new{
                o.Id,
                o.OfferId,
                o.SenderSteamId,
                o.BotId,
                o.ItemsJson,
                o.Status,
                o.ReceivedAt
            }).ToListAsync();
        return Ok(offers);
    }
}