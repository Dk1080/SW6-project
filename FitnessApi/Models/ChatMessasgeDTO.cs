using Microsoft.Extensions.AI;
using MongoDB.Bson;

namespace FitnessApi.Models
{
    public class ChatMessageDTO
    {
        public string? AuthorName { get; set; } 
        public string Role { get; set; }
        public string? Text { get; set; }

        public ChatMessageDTO() { }

        // Constructor to map from the original ChatMessage
        public ChatMessageDTO(ChatMessage message)
        {
            AuthorName = message.AuthorName;
            Role = message.Role.ToString();
            Text = message.Text; 
        }

        public ChatMessageDTO(ChatRole role, string text)
        {
            Role = role.ToString();
            Text = text;
        }


    }
}
