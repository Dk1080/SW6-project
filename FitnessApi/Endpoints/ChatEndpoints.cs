using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

using System.IO;
using System.Linq;

namespace FitnessApi.Endpoints
{
    public static class ChatEndpoints
    {

        public static WebApplication MapChatEndpoints(this WebApplication app)
        {

            //Create list holding chat history
            var chatHistory = new List<ChatMessage>();


            app.MapPost("/chat", async (IChatClient chatClient, ChatDTO chatDTO) =>
            {

                //////Add the messasge to the chat history.
                //chatHistory.Add(new ChatMessage(ChatRole.User, chatDTO.Msg));

                //////Give the chat history to the AI and get the respons

                //ChatResponse response = await chatClient.GetResponseAsync(chatHistory);

                //chatHistory.Add(new ChatMessage(ChatRole.Assistant, response.Text));

                //ChatMessage RrtnMsg = chatHistory.Last();

                //return Results.Ok(RrtnMsg.Text);

            });

            return app;
        }

        public class ChatDTO(string msg)
        {
            public string Msg { get; set; } = msg;
        }
    }
}

