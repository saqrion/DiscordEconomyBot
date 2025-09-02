using Discord;
using Discord.WebSocket;
using DsBot.Commands;

namespace DsBot.Services
{
    public class BotService : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private CommandHandler _commandHandler;

        public BotService(
            DiscordSocketClient client,
            IServiceProvider services,
            IConfiguration config)
        {
            _client = client;
            _services = services;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.Log += msg =>
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            };

            _client.Ready += async () =>
            {
                Console.WriteLine("✅ Bot online!");
                using var scope = _services.CreateScope();
                var interactionHandler = scope.ServiceProvider.GetRequiredService<InteractionHandler>();
                await interactionHandler.InitializeAsync();
            };

            string token = _config["Discord:Token"];
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            using (var scope = _services.CreateScope())
            {
                var voiceTrackingService = scope.ServiceProvider.GetRequiredService<VoiceTrackingService>();
                await voiceTrackingService.InitializeAsync();

                var commands = scope.ServiceProvider.GetServices<IBotCommand>();
                _commandHandler = new CommandHandler(_client, scope.ServiceProvider, commands, _config);
                await _commandHandler.InitializeAsync();
            }

            await Task.Delay(-1, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
