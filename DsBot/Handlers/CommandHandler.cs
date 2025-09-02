using Discord.WebSocket;
using DsBot.Commands;

namespace DsBot.Handlers;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;
    private readonly IEnumerable<IBotCommand> _commands;
    private readonly IConfiguration _config;

    public CommandHandler(DiscordSocketClient client, IServiceProvider services,
        IEnumerable<IBotCommand> commands, IConfiguration config)
    {
        _client = client;
        _services = services;
        _commands = commands;
        _config = config;
    }

    public async Task InitializeAsync()
    {
        _client.Ready += RegisterCommandsAsync;
        _client.SlashCommandExecuted += HandleSlashCommandAsync;
    }

    private async Task RegisterCommandsAsync()
    {
        foreach (var cmd in _commands)
        {
            await _client.Rest.CreateGuildCommand(cmd.BuildCommand(), ulong.Parse(_config["Discord:GuildId"]));
        }
    }

    private async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        var cmd = _commands.FirstOrDefault(c => c.Name == command.CommandName);
        if (cmd != null)
        {
            await cmd.HandleAsync(command, _services);
        }
    }
}
