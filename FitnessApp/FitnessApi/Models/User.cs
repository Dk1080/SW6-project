using MongoDB.EntityFrameworkCore;

namespace FitnessApi.Models
{
    [Collection("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

}
