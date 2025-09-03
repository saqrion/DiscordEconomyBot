using DsBot.Data;
using DsBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DsBot.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BotDbContext _db;

        public UserRepository(BotDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(User user)
        {
            return _db.Users.AddAsync(user).AsTask();
        }

        public Task<User?> GetByDiscordIdAsync(ulong discordUserId)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.DiscordUserId == discordUserId);
        }

        public Task<User?> GetUserWithBalanceAsync(ulong discordUserId)
        {
            return _db.Users
                .Include(u => u.Balances)
                .ThenInclude(b => b.Currency)
                .FirstOrDefaultAsync(u => u.DiscordUserId == discordUserId);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
