using Microsoft.EntityFrameworkCore;
using CS2Storage.Models;

namespace CS2Storage.Data;

public class StorageDbContext : DbContext{
    public StorageDbContext(DbContextOptions<StorageDbContext> options):base(options){
    }

    public DbSet<Bot> Bots => Set<Bot>();
    public DbSet<Deposit> Deposits => Set<Deposit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Bot>().HasIndex(b => b.SteamId).IsUnique();
        modelBuilder.Entity<Deposit>().HasOne(d => d.Bot).WithMany(b => b.Deposits).HasForeignKey(d => d.BotId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Deposit>().HasIndex(d => d.OwnerSteamId);
        modelBuilder.Entity<Deposit>().HasIndex(d => d.AssetId);
    }
}
