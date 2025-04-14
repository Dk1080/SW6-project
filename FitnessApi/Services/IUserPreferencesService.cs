using FitnessApi.Models;
using System.Threading.Tasks;

namespace FitnessApi.Services
{
    public interface IUserPreferencesService
    {
        Task<UserPreferences> GetUserPreferencesAsync(string username);
    }
}