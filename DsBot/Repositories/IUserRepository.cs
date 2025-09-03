using DsBot.Models;

namespace DsBot.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByDiscordIdAsync(ulong discordUserId);
        Task<User?> GetUserWithBalanceAsync(ulong discordUserId);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
