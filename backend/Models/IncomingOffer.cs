namespace CS2Storage.Models;

public class IncomingOffer{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OfferId { get; set; } = string.Empty;
    public string SenderSteamId { get; set; } = string.Empty;
    public Guid BotId { get; set; }
    public Bot Bot { get; set; } = null!;
    public string ItemsJson { get; set; } = "[]";
    public string Status { get; set; } = "Pending";
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}