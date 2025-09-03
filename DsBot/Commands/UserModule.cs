using Discord;
using Discord.Interactions;
using DsBot.Data;
using DsBot.Models;
using DsBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DsBot.Commands
{
    public class UserModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IUserRepository _userRepository;

        public UserModule(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [SlashCommand("balance", "Show your balance")]
        public async Task ShowUserBalanceAsync()
        {
            await DeferAsync(ephemeral: true);

            var user = await _userRepository.GetUserWithBalanceAsync(Context.User.Id);

            if (user == null)
            {
                user = new User() 
                { 
                    DiscordUserId = Context.User.Id,
                    UserName = Context.User.Username
                };
                await _userRepository.AddAsync(user);
            }

            var embed = new EmbedBuilder()
                .WithTitle($"Баланс {user.UserName}")
                .WithColor(Color.Gold)
                .WithTimestamp(DateTimeOffset.Now);

            if (user.Balances == null || !user.Balances.Any())
            {
                embed.WithDescription("Валют пока нет");
            }
            else
            {
                foreach (var balance in user.Balances)
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
