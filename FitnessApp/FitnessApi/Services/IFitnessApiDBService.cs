using FitnessApi.Models;
using MongoDB.Bson;

namespace FitnessApi.Services
{
    public interface IFitnessApiDBService
    {

        IEnumerable<User> GetAllUsers();

        User? GetUserByID(ObjectId id);

        void AddUser(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);

    }
}
