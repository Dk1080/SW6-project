using FitnessApi.Models;
using System.Threading.Tasks;

namespace FitnessApi.Services
{
    public interface IUserPreferencesService
    {
        Task<UserPreferences> GetUserPreferencesAsync(string username);
        Task<UserPreferences> UpdateUserPreferencesAsync(string username, UserPreferences preferences);

    }
}