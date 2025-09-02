using Discord.WebSocket;

namespace DsBot.Services
{
    public class HouseRoleHandler : IHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly HouseService _houseService;

        public HouseRoleHandler(HouseService houseService, DiscordSocketClient client)
        {
            _houseService = houseService;
            _client = client;
        }

        public Task InitializeAsync()
        {
            _client.GuildMemberUpdated += OnGuildMemberUpdatedAsync;
            return Task.CompletedTask;
        }

        private async Task OnGuildMemberUpdatedAsync(Discord.Cacheable<SocketGuildUser, ulong> before, SocketGuildUser after)
        {
            var houseRole = after.Roles.FirstOrDefault(r => _houseService.IsHouseRole(r.Id));

            int? houseId = houseRole != null ? _houseService.GetHouseIdByRole(houseRole.Id) : null;

            await _houseService.AssignHouseAsync(after.Id, houseId);
        }
    }
}
