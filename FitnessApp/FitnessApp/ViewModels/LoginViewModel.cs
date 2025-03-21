using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnesApp;

namespace FitnessApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{

    [ObservableProperty] private string username;
    [ObservableProperty] private string password;
    [ObservableProperty] private string invalidInfo;


    [RelayCommand]
    public async Task SendLoginRequest()
    {
    //    //Reset respone from server
    //    InvalidInfo = string.Empty;

    //    var response = _mainViewModel.SharedClient.PostAsJsonAsync("/login", new UserDto(Username, Password));
    //    response.Wait();
    //    // Console.WriteLine(_mainViewModel.Handler.CookieContainer.SetCookies("http:localhost:8080/test"),);

    //    //TODO Add condition if server is unresponsive / not avalible
    //    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
    //    {
    //        Console.WriteLine(_mainViewModel.Handler.CookieContainer.GetCookies(new Uri("http://localhost:8080/test")));
    //        GoToDashboard();
    //    }
    //    else if (response.Result.StatusCode == System.Net.HttpStatusCode.BadRequest)
    //    {
    //        InvalidInfo = response.Result.Content.ReadAsStringAsync().Result;
    //    }

        await Shell.Current.GoToAsync(nameof(DashboardPage));
    }


    }

    public class UserDto
    {
        public UserDto(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }

    }
