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


        public ObjectId AddChatHistory(ChatHistory chatHistory)
        {
            _databaseContext.ChatHistories.Add(chatHistory);
            _databaseContext.SaveChanges();
            return chatHistory.Id; 
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
            var existingChat = _databaseContext.ChatHistories.FirstOrDefault(c => c.Id == chatHistory.Id);

            if (existingChat != null)
            {
                // Get only new messages that do not already exist
                var newMessages = chatHistory.chatHistory
                    .Where(newMsg => !existingChat.chatHistory.Any(existingMsg =>
                        existingMsg.Role == newMsg.Role &&
                        existingMsg.Text == newMsg.Text))
                    .ToList();

                // Add only the new messages
                existingChat.chatHistory.AddRange(newMessages);

                _databaseContext.Entry(existingChat).State = EntityState.Modified;
                _databaseContext.SaveChanges();
            }
            else
            {
                Console.WriteLine($"ChatHistory with ID {chatHistory.Id} not found.");
            }
        }

        public ChatHistory? GetChatHistoryByID(ObjectId Id)
        {
            return _databaseContext.ChatHistories.FirstOrDefault(x => x.Id == Id);
        }
    }
}
