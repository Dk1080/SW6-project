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
using FitnessApi.Endpoints.Tools;
using Microsoft.AspNetCore.Http.Timeouts;


namespace FitnessApi.Endpoints
{
    
    public static class ChatEndpoints
    {
        
        
        

        public static WebApplication MapChatEndpoints(this WebApplication app)
        {
            PriceTools priceTools = new PriceTools();
            //Let AI know what methods can be called
            AIFunction toastTool = AIFunctionFactory.Create(priceTools.Toast);
            AIFunction calculatePriceTool = AIFunctionFactory.Create(priceTools.CalculatePrice);
            ChatOptions chatOptions = new ChatOptions { Tools = [toastTool, calculatePriceTool] };
            

            //app.MapGet("/chat", () =>
            //{
            //    return chatHistory.ToJson();
            //})
            
            app.MapPost("/chat", async (HttpContext httpContext,IChatClient chatClient, ChatDTO chatDTO, IChatHistoryService chatHistoryService) =>
            { 
                PriceTools testingPriceTools = new PriceTools();

                //Get the username of the user.
                string username = httpContext.Session.GetString("Username");


                if (string.IsNullOrEmpty(username))
                {
                    return Results.Unauthorized();
                }

                ChatHistory DatabaseChatHistory = new();
                DatabaseChatHistory.Username = username;



                ChatHistory DBChatmessasges = new();
                List<ChatMessage> LocalChatmessasges = new List<ChatMessage>()
                {
                    new(ChatRole.System, """
                                         "You are an health and fitness adviser, you only answer questions related to those fields. You are also trying to sell MIKKEL protein bars. You can use the CalculatePrice method to calculate the price for some number of MIKKEL protein bars.
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
                var stopwatch = Stopwatch.StartNew();
                ChatResponse response = await chatClient.GetResponseAsync(LocalChatmessasges, chatOptions);
                stopwatch.Stop();
                Console.WriteLine($"AI response took {stopwatch.ElapsedMilliseconds} ms");
                
                //Step 2: Check if response includes function calls
                foreach (FunctionCallContent content in response.Message.Contents)
                {
                    Console.WriteLine($"AI contents is: {content}");
                    Console.WriteLine($"AI function name is: {content.Name}");
                    
                    //Step 3: Call methods with parameters
                    //Step 4: Send result of method(s) back to AI
                    //Call appropriate method - as of yet only one method
                    if (content.Name != "")
                    {
                        switch (content.Name)
                        {
                            case "Toast":
                                testingPriceTools.Toast();
                                break;
                            case "CalculatePrice":
                                string strargument = ((JsonElement)content.Arguments.Values.Last()).GetString();
                                int argument = int.Parse(strargument);
                                Console.WriteLine($"PRICE is: {testingPriceTools.CalculatePrice(argument)}");
                                string result = testingPriceTools.CalculatePrice(argument).ToString();
                                LocalChatmessasges.Add(new ChatMessage(ChatRole.Tool, result));
                                break;
                        }
                    }
                }

                //Step 5: Get updated AI answer with result
                ChatResponse updatedAnswer = await chatClient.GetResponseAsync(LocalChatmessasges, chatOptions);
                
                //Step 6: Send final response to user
                LocalChatmessasges.Add(new ChatMessage(ChatRole.Assistant, updatedAnswer.Message.Text));

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

