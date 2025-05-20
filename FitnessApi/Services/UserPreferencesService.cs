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

        public async Task<UserPreferences> UpdateUserPreferencesAsync(string username, UserPreferences preferences)
        {
            if (string.IsNullOrEmpty(username) || preferences == null)
            {
                Console.WriteLine($"[Service] Invalid input: username={username}, preferences={(preferences == null ? "null" : "valid")}");
                throw new ArgumentException("Username and preferences cannot be null or empty freak");
            }

            // Find om der allerede er preferences for useren
            var existingPreferences = await _dbContext.UserPreferences.FirstOrDefaultAsync(up => up.User == username);

            // hvis nej lav en ny preference    
            if (existingPreferences == null)
            {
                Console.WriteLine($"[Service] No preferences found for {username}, creating new record.");
                preferences.User = username;

                // Lav id, vil ikke indsætte ind i database uden et id
                if (string.IsNullOrEmpty(preferences.Id))
                {
                    preferences.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                }
                

                _dbContext.UserPreferences.Add(preferences);
            }
            // hvis ja opdater preferences
            else
            {
                Console.WriteLine($"[Service] Updating preferences for {username}.");
                existingPreferences.ChartPreference = preferences.ChartPreference;

                var updatedGoal = preferences.Goals.LastOrDefault();

                if (updatedGoal != null)
                {
                    var newGoal = new Goal
                    {
                        GoalType = updatedGoal.GoalType,
                        Interval = updatedGoal.Interval,
                        Value = updatedGoal.Value,
                        StartDate = DateTime.UtcNow.Date,
                        EndDate = updatedGoal.EndDate
                    };

                    existingPreferences.Goals = new List<Goal> { newGoal };
                }

                // Optionally update other preferences
                existingPreferences.ChartPreference = preferences.ChartPreference;

                // Save the updated document
                //_dbContext.UserPreferences.Update(existingPreferences);

            }

            try
            {
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"[Service] saved preferences for {username} OwO");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"[Service] Error saving preferences for {username}: {ex.Message}");
                throw;
            }

            return existingPreferences ?? preferences;
        }
    }
}