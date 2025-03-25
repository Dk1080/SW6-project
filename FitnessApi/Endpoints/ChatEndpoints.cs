using FitnessApi.Models;
using FitnessApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.IO;
using System.Linq;

namespace FitnessApi.Endpoints
{
    public static class ChatEndpoints
    {

        public static WebApplication MapChatEndpoints(this WebApplication app)
        {
            

            //app.MapGet("/chat", () =>
            //{
            //    return chatHistory.ToJson();
            //});


            app.MapPost("/chat", async (HttpContext httpContext,IChatClient chatClient, ChatDTO chatDTO, IChatHistoryService chatHistoryService) =>
            {

                //Get the username of the user.
                string username = httpContext.Session.GetString("Username");

                ChatHistory DatabaseChatHistory = new();
                DatabaseChatHistory.Username = username;


                //Check if they are logged in.
                if (string.IsNullOrEmpty(username))
                {
                    return Results.Unauthorized();
                }


                ChatHistory DBChatmessasges = new();
                List<ChatMessage> LocalChatmessasges = new List<ChatMessage>();


                //If this is a previous conversation then get it from the database.
                if (chatDTO.Id != ObjectId.Empty)
                {
                    LocalChatmessasges = ChatHistory.ConvertDBToLocal(chatHistoryService.GetChatHistoryByID(chatDTO.Id));
                }


                ////Add the messasge to the chathistory.
                LocalChatmessasges.Add(new ChatMessage(ChatRole.User, chatDTO.Message));


                ////Give the chat history to the AI and get the response.

                ChatResponse response = await chatClient.GetResponseAsync(LocalChatmessasges);

                LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, response.Message.Text));

                ChatMessage RrtnMsg = LocalChatmessasges.Last();


                //Save the conversation to the database
                if(chatDTO.Id == null)
                {
                    chatHistoryService.AddChatHistory(DatabaseChatHistory);
                }
                else
                {
                    //Convert the local list to a DB list
                    DatabaseChatHistory.chatHistory = ChatHistory.ConvertLocalToDb(LocalChatmessasges);
                    chatHistoryService.UpdateChatHistory(DatabaseChatHistory);
                }

                    return Results.Ok(RrtnMsg.Text);

            });

            return app;
        }

        public class ChatDTO
        {
            public string Message { get; set; }
            [BsonRepresentation(BsonType.String)]
            public ObjectId Id { get; set; }
        }





    }
}

