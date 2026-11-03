using System.ComponentModel.DataAnnotations;

namespace CS2Storage.DTOs;

public class CreateBotRequest{
    [Required]
    [RegularExpression(@"^\d{17}$")]
    public string SteamId { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateBotStatusRequest{
    [Required]
    public bool? IsActive { get; set; }
}

public record BotResponse(Guid Id,string SteamId,string Name,bool IsActive);