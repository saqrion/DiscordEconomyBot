using Discord.WebSocket;

namespace DsBot.Models
{
    public class House
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
        public ulong RoleId { get; set; }
    }
}
