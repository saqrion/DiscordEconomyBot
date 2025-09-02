using Discord.WebSocket;
using DsBot.Data;
using DsBot.Models;

namespace DsBot.Services
{
    public class VoiceTrackingService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly Dictionary<ulong, VoiceSession> _activeSessions = new();

        public VoiceTrackingService(DiscordSocketClient client, IServiceProvider services)
        {
            _client = client;
            _services = services;
        }

        public Task InitializeAsync()
        {
            _client.UserVoiceStateUpdated += OnUserVoiceStateUpdate;
            return Task.CompletedTask;
        }

        private async Task OnUserVoiceStateUpdate(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (user.IsBot) return;
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();
            var rewardService = scope.ServiceProvider.GetRequiredService<RewardService>();

            // user join channel
            if (before.VoiceChannel == null && after.VoiceChannel != null)
            {
                var session = new VoiceSession
                {
                    UserId = user.Id,
                    UserName = user.Username,
                    StartTime = DateTime.UtcNow,
                };

                _activeSessions[user.Id] = session;

                Console.WriteLine($"{user.Username} join in {after.VoiceChannel.Name}");
            }
            // user left channel
            else if (before.VoiceChannel != null &&  after.VoiceChannel == null)
            {
                if (_activeSessions.TryGetValue(user.Id, out var session))
                {
                    session.EndTime = DateTime.UtcNow;
                    session.Duration = session.EndTime - session.StartTime;
                    Console.WriteLine($"{user.Username} left {before.VoiceChannel.Name}");
                                    
                    dbContext.Set<VoiceSession>().Add(session);
                    await rewardService.RewarForVoiceSession(session, before.VoiceChannel.Guild.Id, before.VoiceChannel.Users.Count);
                    await dbContext.SaveChangesAsync();

                    _activeSessions.Remove(user.Id);
                }
                
            }
            //user move to another channel
            else if (before.VoiceChannel != null && after.VoiceChannel != null)
            {
                Console.WriteLine($"{user.Username} moved from {before.VoiceChannel.Name} to {after.VoiceChannel.Name}");
            }
        }
    }
}
