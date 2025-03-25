using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Threading;
using System.Xml.Linq;

namespace FitnessApi.Services
{
    public class ChatHistoryService : IChatHistoryService
    {

        private readonly DatabaseContext _databaseContext;

        public ChatHistoryService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        public void AddChatHistory(ChatHistory chatHistory)
        {
            _databaseContext.ChatHistories.Add(chatHistory);
            _databaseContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_databaseContext.ChangeTracker.DebugView.LongView);
            _databaseContext.SaveChanges();
        }

        public void DeleteChatHistory(ChatHistory chatHistory)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChatHistory> GetAllChatHistoriesForAUser(string userName)
        {
            return _databaseContext.ChatHistories.Where(chat => chat.Username == userName).ToList();
        }

        public void UpdateChatHistory(ChatHistory chatHistory)
        {
            _databaseContext.ChatHistories.Update(chatHistory);
            _databaseContext.SaveChanges();
        }

        public ChatHistory? GetChatHistoryByID(ObjectId Id)
        {
            return _databaseContext.ChatHistories.FirstOrDefault(x => x.Id == Id);
        }
    }
}
