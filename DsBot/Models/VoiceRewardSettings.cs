namespace DsBot.Models
{
    public class VoiceRewardSettings
    {
        public int Id { get; set; }
        public double BaseRate { get; set; } = 1;
        public double BonusPerUser { get; set; } = 0;
        public double VipMultiplier { get; set; } = 2;
        public ulong GuildId { get; set; }
    }
}
