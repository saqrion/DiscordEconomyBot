using Discord;
using Discord.WebSocket;
using DsBot.Data;
using DsBot.Services;
using DsBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DsBot.Commands
{
    public class Balance : IBotCommand
    {
        private readonly IServiceProvider _services;
        public string Name => "balance";

        public string Description => "Balance command";

        public Balance(IServiceProvider services)
        {
            _services = services;
        }

        public SlashCommandProperties BuildCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Show your balance")
                .Build();
        }

        public async Task HandleAsync(SocketSlashCommand command, IServiceProvider services)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();

            await command.DeferAsync(ephemeral: true);

            SocketUser user = command.User;
            var userDb = await dbContext.Set<User>()
                .Include(u => u.Balances)
                .ThenInclude(b => b.Currency)
                .FirstOrDefaultAsync(u => u.DiscordUserId == user.Id);

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

            await command.FollowupAsync(embed: embed.Build());
        }

    }
}

