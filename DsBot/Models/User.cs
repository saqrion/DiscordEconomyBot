namespace DsBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public ulong DiscordUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ICollection<Balance> Balances { get; set; }
        public int? HouseId { get; set; }
        public House? House { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
