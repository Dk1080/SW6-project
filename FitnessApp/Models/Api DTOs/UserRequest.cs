namespace FitnessApp.Models.Api_DTOs
{
    public class UserRequest(string username, string password)
    {
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;

    }
}
