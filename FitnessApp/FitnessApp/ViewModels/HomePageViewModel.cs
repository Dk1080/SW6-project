using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessApp.ViewModels;

public partial class HomePageViewModel : ViewModelBase
{

    [ObservableProperty] private string _query = string.Empty;
    [ObservableProperty] private string _response = string.Empty;

    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("http://localhost:8080"),
    };

    [RelayCommand]
    public async Task SendQuery()
    {
        // Check that the query is not empty
        if (string.IsNullOrEmpty(Query))
        {
            Console.WriteLine("Query is empty");
            return;
        }

        // Create the content to send in the POST request
        var content = new StringContent(JsonSerializer.Serialize(new { query = Query }), Encoding.UTF8,
            "application/json");

        // Send the POST request
        HttpResponseMessage response = await sharedClient.PostAsync("/query", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("POST request was successful.");
            Response = await response.Content.ReadAsStringAsync();
        }
        else
        {
            Console.WriteLine($"POST request failed with status code {response.StatusCode}.");
        }
    }
}
    

    
    