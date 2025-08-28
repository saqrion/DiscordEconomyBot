using Discord;
using Discord.Interactions;
using DsBot.Data;
using DsBot.Models;
using DsBot.Services;

namespace DsBot.Commands
{
    [Group("keks", "Admin commands")]
    public class AdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly BotDbContext _botBdContext;
        private readonly VoiceRewardSettingsService _vrSettingsService;

        public AdminModule(BotDbContext botBdContext, VoiceRewardSettingsService vrSettingsService)
        {
            _botBdContext = botBdContext;
            _vrSettingsService = vrSettingsService;
        }

        [SlashCommand("setbaserate", "Set base rate currency per minute")]
        public async Task SetBaseRateAsync(double rate)
        {
            await DeferAsync();
            await _vrSettingsService.UpdateBaseRateAsync(Context.Guild.Id, rate);
            await FollowupAsync($"{rate}");
        }

        [SlashCommand("setbonusperuser", "Set bonus per user in channel")]
        public async Task SetBonusAsync(double bonus)
        {
            await DeferAsync();
            await _vrSettingsService.UpdateBonusAsync(Context.Guild.Id, bonus);
            await FollowupAsync($"Bonus per user in voice channel was changed. New value {bonus}");
        }

        [SlashCommand("setvipmultiplier", "Set vip multiplier")]
        public async Task SetVipMultiplierAsync(double multiplier)
        {
            await DeferAsync();
            await _vrSettingsService.UpdateVipBonusAsync(Context.Guild.Id, multiplier);
            await FollowupAsync($"Vip multiplier was changed. New value {multiplier}");
        }

        [SlashCommand("getrewardsettings", "get current voice reward settings")]
        public async Task GetSettingsAsync()
        {
            await DeferAsync();
            var settings = await _vrSettingsService.GetSettingsAsync(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithTitle($"Voice Reward Settings")
                .WithColor(Color.Teal)
                .WithTimestamp(DateTimeOffset.Now);

            embed.AddField(
                name: "Base rate",
                value: settings.BaseRate,
                inline: false
            );
            embed.AddField(
                name: "Per user bonus",
                value: settings.BonusPerUser,
                inline: false
            );

            embed.AddField(
                name: "Vip multiplier",
                value: settings.VipMultiplier,
                inline: false
            );

            await FollowupAsync(embed: embed.Build(), ephemeral: true);
        }

        [SlashCommand("clear", "Deletes the specified number of chat messages")]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        public async Task DeleteChatMessagesAsync(int amount)
        {
            if (amount <= 0 || amount > 100)
            {
                await RespondAsync("Можно удалить только от 1 до 100 сообщений", ephemeral: true);
                return;
            }

            var channel = Context.Channel as ITextChannel;
            var messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            var deletable = messages.Where(m => (DateTimeOffset.UtcNow - m.Timestamp).TotalDays < 14);

            await channel.DeleteMessagesAsync(deletable);

            await RespondAsync($"Успешно удалено {amount} сообщений", ephemeral:true);
        }               

    }
}
