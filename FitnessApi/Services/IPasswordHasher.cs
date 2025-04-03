namespace FitnessApi.Services
{
    public interface IPasswordHasher
    {

        public string hashPassword(string password);


        public bool verifyPassword(string password, string hashedString);


    }
}
