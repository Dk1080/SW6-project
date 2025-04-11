using DTOs;
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


                if (string.IsNullOrEmpty(username))
                {
                    return Results.Unauthorized();
                }

                ChatHistory DatabaseChatHistory = new();
                DatabaseChatHistory.Username = username;



                ChatHistory DBChatmessasges = new();
                List<ChatMessage> LocalChatmessasges = new List<ChatMessage>();


                //If this is a previous conversation then get it from the database.
                if (ObjectId.Parse(chatDTO.ThreadId) != ObjectId.Empty)
                {
                    LocalChatmessasges = ChatHistory.ConvertDBToLocal(chatHistoryService.GetChatHistoryByID(ObjectId.Parse(chatDTO.ThreadId)));
                }


                ////Add the messasge to the chathistory.
                LocalChatmessasges.Add(new ChatMessage(ChatRole.User, chatDTO.Query));


                ////Give the chat history to the AI and get the response.

                ChatResponse response = await chatClient.GetResponseAsync(LocalChatmessasges);

                LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, response.Messages.First().Text));

                ChatMessage RrtnMsg = LocalChatmessasges.Last();
                ObjectId threadId = new();


                //Save the conversation to the database
                if(ObjectId.Empty == ObjectId.Parse(chatDTO.ThreadId))
                {
                    //Add a new chat and get that threadId
                    DatabaseChatHistory.chatHistory = ChatHistory.ConvertLocalToDb(LocalChatmessasges);
                    threadId = chatHistoryService.AddChatHistory(DatabaseChatHistory);
                }
                else
                {
                    //Convert the local list to a DB list
                    DatabaseChatHistory.chatHistory = ChatHistory.ConvertLocalToDb(LocalChatmessasges);
                    DatabaseChatHistory.Id = ObjectId.Parse(chatDTO.ThreadId);
                    chatHistoryService.UpdateChatHistory(DatabaseChatHistory);

                    threadId = DatabaseChatHistory.Id;

                }

                    return Results.Ok(new ChatDTO(RrtnMsg.Text,threadId,"assistant"));

            });


            app.MapGet("/getChats", (HttpContext httpContext, IChatHistoryService chatHistoryService) =>
            {

                string username = httpContext.Session.GetString("Username");


                if (string.IsNullOrEmpty(username))
                {
                    return Results.Unauthorized();
                }


                Console.WriteLine(username);

                ChatHistoriesResponse chatHistoriesResponse = new();

                //Get from database
                List<ChatHistory> chatHistories = (List<ChatHistory>)chatHistoryService.GetAllChatHistoriesForAUser(username);


                //Convert to ChatHistoryDTO
                foreach(ChatHistory chatHistoryItem in chatHistories)
                {

                    var tmpChat = new ChatHistoryDTO(chatHistoryItem.Id, chatHistoryItem.chatHistory);
                    chatHistoriesResponse.histories.Add(tmpChat);
                }
                //System.NullReferenceException: 'Object reference not set to an instance of an object.'

                return Results.Ok(chatHistoriesResponse);

            });



            return app;
        }







    }
}

