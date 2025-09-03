using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace DsBot.Services
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        public InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services, IConfiguration configuration)
        {
            _client = client;
            _interactionService = interactionService;
            _services = services;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            _client.InteractionCreated += HandleInteraction;

            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

            await _interactionService.RegisterCommandsToGuildAsync(ulong.Parse(_configuration["Discord:GuildId"]));
            //await _interactionService.RegisterCommandsGloballyAsync();

            _interactionService.Log += async (msg) =>
            {
                Console.WriteLine($"[Interaction Service] {msg}");
            };
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
        }


    }
}
