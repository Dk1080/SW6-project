using System.Security.Cryptography;

namespace FitnessApi.Services
{

    /*
     * Function that handels the hashing and verification of passwords with salting.
     * From https://stackoverflow.com/a/73125177
     */

    public class PasswordHasher : IPasswordHasher
    {

        private int saltSize = 16;
        private int keySize = 32;
        private const int iterations = 50000;
        private static readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA256;

        private const char segmentDelimiter = ':';




        public string hashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                algorithm,
                keySize
            );
            return string.Join(
                segmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                iterations,
                algorithm
            );
        }

        public bool verifyPassword(string password,string hashedString)
        {
            string[] segments = hashedString.Split(segmentDelimiter);
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                algorithm,
                hash.Length
            );
            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
}
