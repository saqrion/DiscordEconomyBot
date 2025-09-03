using Discord;
using Discord.WebSocket;
using DsBot.Commands;
using DsBot.Handlers;

namespace DsBot.Services
{
    public class BotService : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private readonly InteractionHandler _interactionHandler;
        private readonly VoiceTrackingService _voiceTrackingService;

        public BotService(
            DiscordSocketClient client,
            IServiceProvider services,
            IConfiguration config,
            InteractionHandler interactionHandler,
            VoiceTrackingService voiceTrackingService)
        {
            _client = client;
            _services = services;
            _config = config;
            _interactionHandler = interactionHandler;
            _voiceTrackingService = voiceTrackingService;
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
                await _interactionHandler.InitializeAsync();
            };

            string token = _config["Discord:Token"];
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await _voiceTrackingService.InitializeAsync();
            using (var scope = _services.CreateScope())
            {
                var commands = scope.ServiceProvider.GetServices<IBotCommand>();
                var _commandHandler = new CommandHandler(_client, scope.ServiceProvider, commands, _config);
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
