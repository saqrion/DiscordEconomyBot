using DsBot.Data;
using DsBot.Models;

namespace DsBot.Services
{
    public class HouseService
    {
        private readonly BotDbContext _dbContext;

        public HouseService(BotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsHouseRole (ulong roleId)
        {
           return _dbContext.Houses.Any(h =>  h.RoleId == roleId);
        }

        public int GetHouseIdByRole(ulong roleId)
        {
            return _dbContext.Houses.First(h => h.RoleId == roleId).Id;
        }

        public async Task AssignHouseAsync(ulong userId, int? houseId)
        {
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                user = new User { DiscordUserId = userId, HouseId = houseId };
                _dbContext.Users.Add(user);
            }
            else
            {
                user.HouseId = houseId;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
