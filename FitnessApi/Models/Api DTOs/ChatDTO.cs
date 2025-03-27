using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FitnessApi.Models.Api_DTOs
{
    public class ChatDTO
    {
        public string Query { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ThreadId { get; set; }

        public string Role { get; set; }

        public ChatDTO() { }

        public ChatDTO(string query, ObjectId threadId, string role)
        {
            Query = query;
            ThreadId = threadId.ToString();
            Role = role;
        }
    }
}
