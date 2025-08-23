using DsBot.Data;
using DsBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DsBot.Services
{
    public class VoiceRewardSettingsService
    {
        private readonly BotDbContext _dbContext;
        public VoiceRewardSettingsService(BotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VoiceRewardSettings> GetSettingsAsync(ulong guildId)
        {
            var settings = await _dbContext.VoiceRewardSettings.FirstOrDefaultAsync(s => s.GuildId == guildId);

            if (settings == null)
            {
                settings = new VoiceRewardSettings()
                {
                    BaseRate = 1,
                    BonusPerUser = 0,
                    VipMultiplier = 0
                };

                _dbContext.VoiceRewardSettings.Add(settings);
                await _dbContext.SaveChangesAsync();
            }

            return settings;
        }

        public async Task UpdateBaseRateAsync(ulong guildId, double rate)
        {
            var settings = await GetSettingsAsync(guildId);
            settings.BaseRate = rate;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateBonusAsync(ulong guildId, double bonus)
        {
            var settings = await GetSettingsAsync(guildId);
            settings.BonusPerUser = bonus;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateVipBonusAsync(ulong guildId, double multiplier)
        {
            var settings = await GetSettingsAsync(guildId);
            settings.VipMultiplier = multiplier;
            await _dbContext.SaveChangesAsync();
        }
    }
}
