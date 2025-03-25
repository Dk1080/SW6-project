using System;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnesApp;
using FitnesApp.Models;
using FitnesApp.Services;

namespace FitnessApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{

    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string invalidInfo;

    //Get RestService
    RestService restService = new();


    [RelayCommand]
    public async Task SendLoginRequest()
    {
        //Check if the username and password are empty
        if (Username == string.Empty || Password == string.Empty) {
            await Application.Current.Windows[0].Page.DisplayAlert("Incomplete login info", "Please fill in the login form!!!", "OK");
            return;
        }

        //Send Login details to server.
        try
        {
            var response = restService._client.PostAsJsonAsync("/login", new User(Username, Password));
            response.Wait();
            //    // Console.WriteLine(_mainViewModel.Handler.CookieContainer.SetCookies("http:localhost:8080/test"),);

            //TODO Add condition if server is unresponsive / not avalible
            if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await Shell.Current.GoToAsync(nameof(DashboardPage));
            }
            else if (response.Result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Unauthorized", "Invalid login details", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        
    }


    }

 
