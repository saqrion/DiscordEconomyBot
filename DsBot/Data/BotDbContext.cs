using DsBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DsBot.Data;

public class BotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Balance> Balances { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<VoiceSession> VoiceSessions { get; set; }
    public DbSet<VoiceRewardSettings> VoiceRewardSettings { get; set; }
    public DbSet<House> Houses { get; set; }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.DiscordUserId)
            .IsUnique();

        modelBuilder.Entity<Balance>()
            .HasOne(b => b.User)
            .WithMany(u => u.Balances)
            .HasForeignKey(b => b.UserId);

        modelBuilder.Entity<Balance>()
            .HasOne(b => b.Currency)
            .WithMany(c => c.Balances)
            .HasForeignKey(b => b.CurrencyId);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.ToUser)
            .WithMany()
            .HasForeignKey(t => t.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.FromUser)
            .WithMany()
            .HasForeignKey(t => t.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
