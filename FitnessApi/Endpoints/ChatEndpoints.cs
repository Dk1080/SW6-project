
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
            AIFunction SetPreferencesAndGoalsTool = AIFunctionFactory.Create(databaseTools.SetPreferencesAndGoals);
            AIFunction UpdateGoalTool = AIFunctionFactory.Create(databaseTools.UpdateGoal);
            ChatOptions chatOptions = new ChatOptions { Tools = [GetFitnessDataTool, SetPreferencesAndGoalsTool, UpdateGoalTool] };
            
            app.MapPost("/chat", async (HttpContext httpContext,IChatClient chatClient, ChatDTO chatDTO, IChatHistoryService chatHistoryService, IHealthDataService healthDataService, IUserPreferencesService userPreferencesService) =>
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
                                         "You are an health and fitness adviser, you only answer questions related to those fields. You do not always need to call functions, only when the user provides the necessary data
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

                bool functioncallFlag = false;

                foreach (AIContent content in response.Messages.First().Contents)
                {
                    if (content is FunctionCallContent)
                    {
                        FunctionCallContent functionCallContent = content as FunctionCallContent;
                        
                        Console.WriteLine($"AI contents is: {functionCallContent}");
                        Console.WriteLine($"AI function name is: {functionCallContent.Name}");
                    
                        //Step 3: Call methods with parameters
                        //Step 4: Send result of method(s) back to AI
                        //Call appropriate method - as of yet only one method
                        string result = "";
                        if (functionCallContent.Name != "")
                        {
                            switch (functionCallContent.Name)
                            {
                                case "GetFitnessData":
                                    result = databaseTools.GetFitnessData(healthdata);
                                    Console.WriteLine($"Fitness data is: {result}");
                                    break;

                                case "SetPreferencesAndGoals":
                                    functionCallContent.Arguments.TryGetValue("chartPreference", out var chartPreferenceObj);
                                    functionCallContent.Arguments.TryGetValue("goalType", out var goalTypeObj);
                                    functionCallContent.Arguments.TryGetValue("value", out var valueObj);
                                    functionCallContent.Arguments.TryGetValue("interval", out var intervalObj);
                                    functionCallContent.Arguments.TryGetValue("endDate", out var endDateObj);

                                    // Convert to string, default to "none" if null
                                    string chartPreference = chartPreferenceObj?.ToString() ?? "none";
                                    string goalType = goalTypeObj?.ToString() ?? "none";
                                    string value = valueObj?.ToString() ?? "none";
                                    string interval = intervalObj?.ToString() ?? "none";
                                    string endDate = endDateObj?.ToString() ?? "none";

                                    result = await databaseTools.SetPreferencesAndGoals(
                                        userPreferencesService,
                                        username,
                                        chartPreference,
                                        goalType,
                                        value,
                                        interval,
                                        endDate);
                                    break;

                                case "UpdateGoal":
                                    result = await databaseTools.UpdateGoal(userPreferencesService,
                                        username, 
                                        functionCallContent.Arguments["goalType"].ToString(), 
                                        functionCallContent.Arguments["value"].ToString(), 
                                        functionCallContent.Arguments["interval"].ToString(), 
                                        functionCallContent.Arguments["endDate"].ToString());
                                    break;


                            }
                        }
                        LocalChatmessasges.Add(new ChatMessage(ChatRole.Tool, result));
                        
                        //Step 5: Get updated AI answer with result
                        ChatResponse updatedAnswer = await chatClient.GetResponseAsync(LocalChatmessasges, chatOptions);
                
                        //Step 6: Send final response to user
                        LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, updatedAnswer.Messages.First().Text));
                        
                        functioncallFlag = true;
                    }
                    
                }

                if (!functioncallFlag)
                {
                    LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, response.Messages.First().Text));
                }

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


                //Convert to ChatHistoryDTO and remove tool call messasges
                foreach (ChatHistory chatHistoryItem in chatHistories)
                {
                    chatHistoryItem.chatHistory.RemoveAll(chat => chat.Role == "tool" || chat.Role == "system");

                    var tmpChat = new ChatHistoryDTO(chatHistoryItem.Id, chatHistoryItem.chatHistory);
                    chatHistoriesResponse.histories.Add(tmpChat);
                }
                //System.NullReferenceException: 'Object reference not set to an instance of an object.'

                //Check if preferences and goals are set
                var preferencesAndGoals = await userPreferencesService.GetUserPreferencesAsync(username);
                bool promptNewGoal = false;
                Goal goal = null;

                if (preferencesAndGoals?.Goals?.Any() == true)
                {
                    goal = preferencesAndGoals.Goals.FirstOrDefault();
                    if (goal != null)
                    {
                        var endDate = goal.EndDate.Date;
                        var today = DateTime.UtcNow.Date;
                        var expiredGoalDate = (endDate - today).Days;
                        if (expiredGoalDate < 0)
                        {
                            promptNewGoal = true;
                        }
                    }
                }

                if (promptNewGoal) 
                {
                    ChatHistory chatHistoryGoal = new();
                    chatHistoryGoal.chatHistory = new List<ChatMessageDTO>();
                    chatHistoryGoal.chatHistory.Add(new ChatMessageDTO(ChatRole.Assistant, 
                        "Hello!" +
                        "\nIt seems like your goal has expired, please set a new goal!" +
                        "\nPlease tell me what your health and/or fitness goals are!" +
                        "\nWhich interval would you like in your overview?(weekly, biweekly or monthly)" +
                        "\nWhat date would you like the goal to end?(yyyy-mm-dd format)"));
                    chatHistoryGoal.Username = username;
                    //Get threadID
                    chatHistoryGoal.Id = chatHistoryService.AddChatHistory(chatHistoryGoal);

                    //Add to top of return messages.
                    chatHistoriesResponse.histories.Add(new ChatHistoryDTO(chatHistoryGoal.Id, chatHistoryGoal.chatHistory));

                }


                //Start chat 

                if (preferencesAndGoals == null)
                {
                    //Create new chathistory to insert into database to get threadId.
                    ChatHistory chatHistoryGoal = new();
                    chatHistoryGoal.chatHistory = new List<ChatMessageDTO>();
                    chatHistoryGoal.chatHistory.Add(new ChatMessageDTO(ChatRole.Assistant, 
                        "Hello, I am your health and fitness AI advisor!" +
                        "\nLet's start by setting up some goals and preferences for you." +
                        "\nPlease tell me what your health and/or fitness goals are!" +
                        "\nWhich interval would you like in your overview?(weekly, biweekly or monthly)" +
                        "\nWhat date would you like the goal to end?(yyyy-mm-dd format)" +
                        "\nHow would you like your graphs displayed?(Circle or Column)"));
                    chatHistoryGoal.Username = username;
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

