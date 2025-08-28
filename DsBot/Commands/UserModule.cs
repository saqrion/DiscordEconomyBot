using Discord;
using Discord.Interactions;
using DsBot.Data;
using DsBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DsBot.Commands
{
    public class UserModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly BotDbContext _botDbContext;

        public UserModule(BotDbContext botDbContext)
        {
            _botDbContext = botDbContext;
        }

        [SlashCommand("balance", "Show your balance")]
        public async Task ShowUserBalanceAsync()
        {
            await DeferAsync(ephemeral: true);

            var userDb = await _botDbContext.Set<User>()
                .Include(u => u.Balances)
                .ThenInclude(b => b.Currency)
                .FirstOrDefaultAsync(u => u.DiscordUserId == Context.User.Id);

            var embed = new EmbedBuilder()
                .WithTitle($"Баланс {userDb.UserName}")
                .WithColor(Color.Gold)
                .WithTimestamp(DateTimeOffset.Now);

            if (userDb.Balances == null || !userDb.Balances.Any())
            {
                embed.WithDescription("Валют пока нет");
            }
            else
            {
                foreach (var balance in userDb.Balances)
                {
                    embed.AddField(
                        name: balance.Currency.Name,
                        value: $"{balance.Currency.Symbol} {balance.Amount}",
                        inline: true
                    );
                }
            }

            await FollowupAsync(embed: embed.Build());
        }
    }
}
