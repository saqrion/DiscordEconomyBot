namespace DsBot.Models
{
    public class VoiceSession
    {
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
