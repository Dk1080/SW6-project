namespace FitnessApp.Models.Api_DTOs
{
    public class ChatMessageDTO(string role, string text)
    {
        public string? AuthorName { get; set; }
        public string Role { get; set; } = role;
        public string? Text { get; set; } = text;


    }
}
