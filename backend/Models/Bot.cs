namespace CS2Storage.Models;

public class Bot{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SteamId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}
