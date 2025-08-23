using Discord.WebSocket;
using Discord;

namespace DsBot.Commands
{
    public interface IBotCommand
    {
        string Name { get; }
        string Description { get; }
        SlashCommandProperties BuildCommand();
        Task HandleAsync(SocketSlashCommand command, IServiceProvider services);
    }
}
