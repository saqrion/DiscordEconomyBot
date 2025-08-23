using Discord;
using Discord.WebSocket;
using DsBot.Services;

namespace DsBot.Commands
{
    public class AdminCommands : IBotCommand
    {
        public string Name => "admin";

        public string Description => "Admin commands";

        public SlashCommandProperties BuildCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription(Description)
                .AddOption("setbaserate", ApplicationCommandOptionType.Number, "Set base rate currency per minute", isRequired: true)
                .AddOption("setbonusperuser", ApplicationCommandOptionType.Number, "Set bonus per user in channel", isRequired: true)
                .AddOption("setvipmultiplier", ApplicationCommandOptionType.Number, "Set vip multiplier", isRequired: true)
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .Build();
        }

        public async Task HandleAsync(SocketSlashCommand command, IServiceProvider services)
        {
            var subCommand = command.Data.Options.First();
            var vrService = services.GetRequiredService<VoiceRewardSettingsService>();

            switch (subCommand.Name)
            {
                case "setbaserate":
                    var rate = subCommand.Value;
                    await vrService.UpdateBaseRateAsync(command.GuildId!.Value, (double)rate);
                    await command.RespondAsync($"Base rate was changed. New value {rate} per minute"); 
                    break;
                case "setbonusperuser":
                    var bonus = subCommand.Value;
                    await vrService.UpdateBonusAsync(command.GuildId!.Value, (double)bonus);
                    await command.RespondAsync($"Bonus per user in voice channel was changed. New value {bonus}");
                    break;
                case "setvipmultiplier":
                    var multiplier = subCommand.Value;
                    await vrService.UpdateVipBonusAsync(command.GuildId!.Value, (double)multiplier);
                    await command.RespondAsync($"Vip multiplier was changed. New value {multiplier}");
                    break;
            }
        }
    }
}
