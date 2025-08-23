namespace DsBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public ulong DiscordUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Balance> Balances { get; set; }
    }
}
