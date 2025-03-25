using FitnessApi.Models;
using MongoDB.Bson;

namespace FitnessApi.Services
{
    public interface IChatHistoryService
    {

        ChatHistory? GetChatHistoryByID(ObjectId id);


        public IEnumerable<ChatHistory> GetAllChatHistoriesForAUser(string UserName);


        void AddChatHistory(ChatHistory chatHistory);

        void UpdateChatHistory(ChatHistory chatHistory);

        void DeleteChatHistory(ChatHistory chatHistory);


    }
}
