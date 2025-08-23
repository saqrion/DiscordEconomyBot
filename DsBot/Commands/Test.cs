using Discord;
using Discord.WebSocket;
using DsBot.Services;

namespace DsBot.Commands
{
    public class Test : IBotCommand
    {
        public string Name => "test";

        public string Description => "test";

        public SlashCommandProperties BuildCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("TEST")
                .Build();
        }

        public async Task HandleAsync(SocketSlashCommand command, IServiceProvider services)
        {
            using var scope = services.CreateScope();

            SocketUser user = command.User;

            var userId = user.Id;
            var username = user.Username;
            var tag = user.Discriminator;
            var avatarUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            await command.RespondAsync($"Привет {username}#{tag}! Вот твой аватар: {avatarUrl}");
        }

    }
}

