using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace FitnessApi.Services
{
    public class UserService : IUserService
    {

        private readonly DatabaseContext _databaseContext;

        public UserService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public void AddUser(User user)
        {
            _databaseContext.Users.Add(user);

            _databaseContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_databaseContext.ChangeTracker.DebugView.LongView);

            _databaseContext.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _databaseContext.Users.OrderBy(x => x.Id).Take(20).AsNoTracking().AsEnumerable();
        }

        public User? GetUserByID(ObjectId id)
        {
            return _databaseContext.Users.FirstOrDefault(x => x.Id == id);
        }


        public User? GetUserByName(String username)
        {
            return _databaseContext.Users.FirstOrDefault(x => x.Username == username);
        }



        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
