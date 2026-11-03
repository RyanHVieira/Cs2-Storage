using System.ComponentModel.DataAnnotations;

namespace CS2Storage.DTOs;

public class IncomingOfferRequest{
    [Required]
    [RegularExpression(@"^\d{17}$")]
    public string BotSteamId { get; set; } = string.Empty;
    [Required]
    public string OfferId { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^\d{17}$")]
    public string SenderSteamId { get; set; } = string.Empty;
    [Required]
    [MinLength(1)]
    public List<IncomingOfferItemRequest> Items { get; set; } = [];
}

public class IncomingOfferItemRequest{
    [Required]
    public string AssetId { get; set; } = string.Empty;
    [Required]
    public string ClassId { get; set; } = string.Empty;
    [Required]
    public string InstanceId { get; set; } = string.Empty;
    [Required]
    public string MarketHashName { get; set; } = string.Empty;
    public int AppId { get; set; }
    [Required]
    public string ContextId { get; set; } = string.Empty;
}