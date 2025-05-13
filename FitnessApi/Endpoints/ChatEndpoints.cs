
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
using System.Text.RegularExpressions;
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
            ChatOptions chatOptions = new ChatOptions { Tools = [GetFitnessDataTool, SetPreferencesAndGoalsTool] };
            
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
                                         "You are an health and fitness adviser, you only answer questions related to those fields. 
                                         If you need to call a tool, respond with one of the following depending on the scenario:
                                         If the user wants to have their fitness data listed or you want to analyze it respond with:
                                         [tool: GetFitnessData]
                                         or
                                         If the user wants to set their preferences and goals, get values for all the parameters and respond with this:
                                         [tool: SetPreferencesAndGoals]
                                         {
                                            "chartPreference":"value1",
                                            "goalType":"value2"
                                            "value":"value3",
                                            "interval":"value4",
                                            "endDate":"value5"
                                         }
                                         where value1, value2, value3, value4 and value5 are placeholders for whatever values the user wants. Dont respond with this before you have received all the values.
                                         or
                                         If the user wants to update any of goal type, goal value, goal interval, goal end date or all of them, respond with this only if you have received all the value:
                                         [tool: UpdateGoal]
                                         {
                                            "goalType":"value1",
                                            "value":"value2",
                                            "interval":"value3",
                                            "endDate":"value4"      
                                         }
                                         where value1, value2, value3 and value4 are placeholders for whatever values the user wants. Your answer may not contain the string "[tool: UpdateGoal]"
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
                ChatResponse response = await chatClient.GetResponseAsync(LocalChatmessasges);
                
                //Step 2: Check if response includes function calls
                
                Console.WriteLine($"Contents: {response.Messages.First().Contents}");
                Console.WriteLine($"Text is {response.Messages.First().Text}");
                LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, response.Messages.First().Text));

                if(response.Messages.First().Text.Contains("[tool: "))
                {
                    //Retrieve what tool to call
                    string tool = ExtractToolName(response.Messages.First().Text);
                    
                    //Step 3: Call methods with parameters
                    //Step 4: Send result of method(s) back to AI
                    //Call appropriate method - as of yet only one method
                    string result = "";
                    if (tool != "")
                    {
                        //Get any input parameter values
                        
                        switch (tool)
                        {
                            case "GetFitnessData":
                                result = databaseTools.GetFitnessData(healthdata);
                                Console.WriteLine($"Fitness data is: {result}");
                                break;
                            
                            case "SetPreferencesAndGoals":
                                var parameters = ExtractJsonPayload(response.Messages.First().Text);
                                foreach (var VARIABLE in parameters.EnumerateObject())
                                {
                                    Console.WriteLine($"Key: {VARIABLE.Name}, Value: {VARIABLE.Value}");
                                }
                                result = await databaseTools.SetPreferencesAndGoals(userPreferencesService, username, parameters.GetProperty("chartPreference").ToString(), parameters.GetProperty("goalType").ToString(), parameters.GetProperty("value").ToString(), parameters.GetProperty("interval").ToString(), parameters.GetProperty("endDate").ToString());
                                break;

                            case "UpdateGoal":
                                var parametersUpdateGoal = ExtractJsonPayload(response.Messages.First().Text);
                                result = await databaseTools.UpdateGoal(userPreferencesService, username, parametersUpdateGoal.GetProperty("goalType").ToString(), parametersUpdateGoal.GetProperty("value").ToString(), parametersUpdateGoal.GetProperty("interval").ToString(), parametersUpdateGoal.GetProperty("endDate").ToString());
                                break;
                        }
                    }
                    LocalChatmessasges.Add(new ChatMessage(ChatRole.Tool, result));
                    
                    //Step 5: Get updated AI answer with result
                    ChatResponse updatedAnswer = await chatClient.GetResponseAsync(LocalChatmessasges);
                
                    //Step 6: Send final response to user
                    LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, updatedAnswer.Messages.First().Text));
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


                //Convert to ChatHistoryDTO
                foreach (ChatHistory chatHistoryItem in chatHistories)
                {

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
                    goal = preferencesAndGoals.Goals.FirstOrDefault(g => g.GoalType == "steps");
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
                        "\nHow would you like your graphs displayed?(Halfcircle or Column)"));
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
        
        static string ExtractToolName(string input)
        {
            var match = Regex.Match(input, @"\[tool:\s*(.*?)\]");
            return match.Success ? match.Groups[1].Value : null;
        }
        
        static JsonElement ExtractJsonPayload(string input)
        {
            var match = Regex.Match(input, @"\[tool:\s*\w+\s*\]\s*(\{.*\})", RegexOptions.Singleline);
            if (!match.Success)
                throw new ArgumentException("No valid tool payload found.");

            string json = match.Groups[1].Value;

            var document = JsonDocument.Parse(json);
            return document.RootElement.Clone(); // Clone to avoid using disposed document
        }
    }
}

