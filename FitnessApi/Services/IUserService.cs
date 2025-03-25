using FitnessApi.Models;
using MongoDB.Bson;

namespace FitnessApi.Services
{
    public interface IUserService
    {

        IEnumerable<User> GetAllUsers();

        User? GetUserByID(ObjectId id);

        User? GetUserByName(String name);


        void AddUser(User user);

        void UpdateUser(User user);

        void DeleteUser(User user);

    }
}
