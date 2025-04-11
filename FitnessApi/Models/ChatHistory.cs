using DTOs;
using Microsoft.Extensions.AI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Models
{
    [Collection("ChatHistories")]
    public class ChatHistory
    {
        [BsonId]
        public ObjectId Id { get; set; }


        [Required(ErrorMessage = "User has to have a name!")]
        [Display(Name = "Username")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Has to have a ChatHistory!")]
        [Display(Name = "ChatHistory")]
        public List<ChatMessageDTO> chatHistory { get; set; }


        static public List<ChatMessage> ConvertDBToLocal(ChatHistory chatHistoryDB)
        {

            List<ChatMessage> rtnList = new List<ChatMessage>();

            foreach (ChatMessageDTO messasge in chatHistoryDB.chatHistory)
            {
                rtnList.Add(new ChatMessage(new ChatRole(messasge.Role), messasge.Text));
            }
            return rtnList;

        }
        static public List<ChatMessageDTO> ConvertLocalToDb(List<ChatMessage> LocalchatMessages)
        {

            List<ChatMessageDTO> rtnList = new List<ChatMessageDTO>();

            foreach (ChatMessage messasge in LocalchatMessages)
            {
                rtnList.Add(new ChatMessageDTO(messasge.Role, messasge.Text));
            }

            return rtnList;

        }


    }

}
