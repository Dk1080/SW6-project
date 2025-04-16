
using System.ComponentModel;
using System.Diagnostics;
using FitnessApi.Models;
using System.ComponentModel;
using System.Diagnostics;
using FitnessApi.Models;
using FitnessApi.Models.Api_DTOs;

using FitnessApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.IO;
using System.Linq;
using System.Text.Json;
using Amazon.Auth.AccessControlPolicy;
using DTOs;
using FitnessApi.Endpoints.Tools;
using Microsoft.AspNetCore.Http.Timeouts;


namespace FitnessApi.Endpoints
{
    
    public static class ChatEndpoints
    {
        
        
        

        public static WebApplication MapChatEndpoints(this WebApplication app)
        {
            DatabaseTools databaseTools = new DatabaseTools();
            //Let AI know what methods can be called
            AIFunction GetFitnessDataTool = AIFunctionFactory.Create(databaseTools.GetFitnessData);
            ChatOptions chatOptions = new ChatOptions { Tools = [GetFitnessDataTool] };
            
            app.MapPost("/chat", async (HttpContext httpContext,IChatClient chatClient, ChatDTO chatDTO, IChatHistoryService chatHistoryService, IHealthDataService healthDataService) =>
            { 
                DatabaseTools databaseTools = new DatabaseTools();

                //Get the username of the user.
                string username = httpContext.Session.GetString("Username");


                if (string.IsNullOrEmpty(username))
                {
                    return Results.Unauthorized();
                }

                HealthInfo healthdata = healthDataService.GetHealthInfoForUser(username);

                
                ChatHistory DatabaseChatHistory = new();
                DatabaseChatHistory.Username = username;
                
                ChatHistory DBChatmessasges = new();
                List<ChatMessage> LocalChatmessasges = new List<ChatMessage>()
                {
                    new(ChatRole.System, """
                                         "You are an health and fitness adviser, you only answer questions related to those fields. Say hello to the user before they say anything.
                                         """)
                };
 


                //If this is a previous conversation then get it from the database.
                if (ObjectId.Parse(chatDTO.ThreadId) != ObjectId.Empty)
                {
                    LocalChatmessasges = ChatHistory.ConvertDBToLocal(chatHistoryService.GetChatHistoryByID(ObjectId.Parse(chatDTO.ThreadId)));
                }


                ////Add the messasge to the chathistory.
                LocalChatmessasges.Add(new ChatMessage(ChatRole.User, chatDTO.Query));


                ////Give the chat history to the AI and get the response.
                //Step 1: User sends message
                ChatResponse response = await chatClient.GetResponseAsync(LocalChatmessasges, chatOptions);
                
                //Step 2: Check if response includes function calls

                foreach (FunctionCallContent content in response.Messages.First().Contents)
                {
                    Console.WriteLine($"AI contents is: {content}");
                    Console.WriteLine($"AI function name is: {content.Name}");
                    
                    //Step 3: Call methods with parameters
                    //Step 4: Send result of method(s) back to AI
                    //Call appropriate method - as of yet only one method
                    string result = "";
                    if (content.Name != "")
                    {
                        switch (content.Name)
                        {
                            case "GetFitnessData":
                                result = databaseTools.GetFitnessData(healthdata);
                                Console.WriteLine($"Fitness data is: {result}");

                                break;
                        }
                    }
                    LocalChatmessasges.Add(new ChatMessage(ChatRole.Tool, result));
                }

                //Step 5: Get updated AI answer with result
                ChatResponse updatedAnswer = await chatClient.GetResponseAsync(LocalChatmessasges, chatOptions);
                
                //Step 6: Send final response to user
                LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, updatedAnswer.Messages.First().Text));


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
            

            app.MapGet("/getChats", async (HttpContext httpContext, IChatHistoryService chatHistoryService, IChatClient chatClient, IUserPreferencesService userPreferencesService) =>
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
                
                //Check if preferences and goals are set
                var preferencesAndGoals = await userPreferencesService.GetUserPreferencesAsync(username);
                //Start chat 
                
                if (preferencesAndGoals == null)
                {
                    //Create new chathistory to insert into database to get threadId.
                    ChatHistory chatHistoryGoal = new();
                    chatHistoryGoal.chatHistory = new List<ChatMessageDTO>();
                    chatHistoryGoal.chatHistory.Add(new ChatMessageDTO(ChatRole.Assistant, "Hello, I am your health and fitness AI advisor!\nLet's start by setting up some goals and preferences for you.\nPlease tell me what your health and/or fitness goals are!\nAnd by the way, how do you prefer your graphs?"));

                    //Get threadID
                    chatHistoryGoal.Id = chatHistoryService.AddChatHistory(chatHistoryGoal);

                    //Add to top of return messages.
                    chatHistoriesResponse.histories.Add(new ChatHistoryDTO(chatHistoryGoal.Id, chatHistoryGoal.chatHistory));
                }
                return Results.Ok(chatHistoriesResponse);
            });
            return app;
        }
    }
}

