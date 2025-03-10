using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp.Views;


namespace FitnessApp.ViewModels;


public partial class ChatBotViewModel : ViewModelBase
{
    
    
    private readonly MainViewModel _mainViewModel;
    
    [ObservableProperty] private string _query = string.Empty;
    [ObservableProperty] private string _response = string.Empty;

    //Create and specify settings for websocket connection.
    private SocketsHttpHandler _handler = new();
    private ClientWebSocket _ws = new();
    

    //Create collection holding chat records.
    public ObservableCollection<ChatBubble> Chat { get; set; } = new();
    
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("http://localhost:8080"),
    };
    
    public ChatBotViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        ConnectWebSocketAsync().ConfigureAwait(false);
    }

    private async Task ConnectWebSocketAsync()
    {
        try
        {
            //Comment or uncomment based OS the app is running.
            //string wsAddress =  "ws://10.0.2.2:8080/ws";
            string wsAddress = "ws://localhost:8080/ws";
            
            await _ws.ConnectAsync(new Uri(wsAddress), new HttpMessageInvoker(_handler), CancellationToken.None);
            Console.WriteLine("Connected to WebSocket!");
            ReceiveChat().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket connection error: {ex.Message}");
        }
    }


    [RelayCommand]
    public async Task ReceiveChat()
    {
        var buffer = new ArraySegment<byte>(new byte[1024]);
    
        while (_ws.State == WebSocketState.Open)
        {
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await _ws.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage); // Continue reading if the message is not complete

                string receivedMessage = Encoding.UTF8.GetString(ms.ToArray());
                Console.WriteLine("Message received: " + receivedMessage);

                // Add the response to the UI
                Chat.Add(new ChatBubble(receivedMessage, false));
            }
        }
    }

    

    [RelayCommand]
    public async Task SendQuery()
    {
        // Check that the query is not empty
        if (string.IsNullOrEmpty(Query))
        {
            Console.WriteLine("Query is empty");
            return;
        }
        
        // Sending the query to the server.
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(Query));
        await _ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine("Message sent!");
        
        //Add the query to the UI
        Chat.Add(new ChatBubble(Query,true));
    }
    
    [RelayCommand]
    public void GoToDashboard()
    {
        _mainViewModel.NavigateTo(new DashboardViewModel(_mainViewModel));
        //Close WS connection when leaving view.
        _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).ConfigureAwait(false);
    }

}


public class ChatBubble(string text, bool isUser)
{
    public string text { get; set; } = text;
    public bool IsUser { get; set; } = isUser;
    
}
    
    