using FitnessApi.Models;
using MongoDB.Bson;

namespace FitnessApi.Services
{
    public class FitnessApiDBService : IFitnessApiDBService
    {

        private readonly DatabaseContext _databaseContext;

        public FitnessApiDBService(DatabaseContext databaseContext)
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
            throw new NotImplementedException();
        }

        public User? GetUserByID(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
