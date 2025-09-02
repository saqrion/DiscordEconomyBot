using DsBot.Data;
using DsBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DsBot.Services
{
    public class RewardService
    {
        private readonly BotDbContext _dbContext;
        private readonly IServiceProvider _services;

        public RewardService(BotDbContext dbContext, IServiceProvider services)
        {
            _dbContext = dbContext;
            _services = services;
        }

        public async Task RewarForVoiceSession (VoiceSession session, ulong guildId, int usersInChannel)
        {            
            var user = await _dbContext.Set<User>().Include(u => u.Balances)
                .FirstOrDefaultAsync(u => u.DiscordUserId == session.UserId);

            if (user == null)
            {
                user = new User
                {
                    DiscordUserId = session.UserId,
                    UserName = session.UserName,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Set<User>().Add(user);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("New User saved");
            }
            
            var currency = await _dbContext.Set<Currency>().FirstOrDefaultAsync();
            if (currency == null)
            {
                currency = new Currency { Name = "Coins", Symbol = "💰" };
                _dbContext.Add(currency);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("Default currency saved");
            }

            // for test 1 minute = 1 coin

            var vrService = _services.GetRequiredService<VoiceRewardSettingsService>();
            var settings = await vrService.GetSettingsAsync(guildId);

            double reward = session.Duration.TotalSeconds * settings.BaseRate / 60;
            reward += (settings.BonusPerUser / 100) * (usersInChannel - 1);

            if (reward <= 0) return;

            var balance = await _dbContext.Set<Balance>().Include(b => b.User)
                .FirstOrDefaultAsync(b => b.User.DiscordUserId == session.UserId && b.CurrencyId == currency.Id);

            if (balance == null)
            {
                balance = new Balance
                {
                    UserId = user.Id,
                    CurrencyId = currency.Id,
                    Amount = 0
                };
                _dbContext.Set<Balance>().Add(balance);
            }
           
            balance.Amount += reward;
            await _dbContext.SaveChangesAsync();

        }
    }
}
