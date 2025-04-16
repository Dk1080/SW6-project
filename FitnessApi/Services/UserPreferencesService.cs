using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FitnessApi.Services
{
    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly DatabaseContext _dbContext;

        public UserPreferencesService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserPreferences> GetUserPreferencesAsync(string username)
        {
            var result = await _dbContext.UserPreferences
                .FirstOrDefaultAsync(up => up.User == username);
            Console.WriteLine($"[Service] Found preferences for {username}: {result != null}");
            return result;
        }
    }
}