using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace FitnessApp.Models.Api_DTOs
{
    public class ChatHistoryDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public List<ChatMessageDTO> ChatHistory { get; set; } = new List<ChatMessageDTO>();


        public ChatHistoryDTO() { }


        public ChatHistoryDTO(ObjectId id, List<ChatMessageDTO> chatHistory)
        {
            Id = id.ToString();  // Convert ObjectId to string
            ChatHistory = chatHistory;
        }
    }
}
