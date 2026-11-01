namespace CS2Storage.Models;

public class Deposit{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AssetId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string InstanceId { get; set; } = string.Empty;
    public string MarketHashName { get; set; } = string.Empty;
    public string OwnerSteamId { get; set; } = string.Empty;
    public Guid BotId { get; set; }
    public Bot Bot { get; set; } = null!;
    public DateTime DepositedAt { get; set; } = DateTime.UtcNow;
    public DateTime? WithdrawnAt { get; set; }
    public string Status { get; set; } = "Stored";
}
