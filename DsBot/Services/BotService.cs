using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DsBot.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DsBot.Services
{
    public class BotService : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private readonly VoiceTrackingService _voiceTrackingService;
        private  CommandHandler _commandHandler;
        private readonly InteractionHandler _interactionHandler;


        public BotService(DiscordSocketClient client, IServiceProvider services, 
            IConfiguration config, VoiceTrackingService voiceTrackingService, InteractionHandler interactionHandler)
        {
            _services = services;
            _config = config;
            _client = client;
            _voiceTrackingService = voiceTrackingService;
            _interactionHandler = interactionHandler;
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

            var commands = _services.GetServices<IBotCommand>();
            _commandHandler = new CommandHandler(_client, _services, commands, _config);
            await _commandHandler.InitializeAsync();
            //await _interactionHandler.InitializeAsync();

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
