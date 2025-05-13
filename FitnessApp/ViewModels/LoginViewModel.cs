using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp;
using FitnessApp.Services.APIs;
using DTOs;
using Microsoft.Maui.Controls;

namespace FitnessApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string username = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string invalidInfo = string.Empty;

    private IUserLoginApi _userLoginApi;
    private readonly CookieContainer _cookieContainer;
    private readonly IPopupService _popupService;



    public LoginViewModel(IUserLoginApi userLoginApi, CookieContainer cookieContainer, IPopupService popupService)
    {
        _userLoginApi = userLoginApi;
        _cookieContainer = cookieContainer;
        _popupService = popupService;

    }

    [RelayCommand]
    public async Task SendLoginRequest()
    {
        //Check if the username and password are empty
        if (Username == string.Empty || Password == string.Empty)
        {
            await Application.Current.Windows[0].Page.DisplayAlert("Incomplete login info", "Please fill in the login form!!!", "OK");
            return;
        }


        //Send Login details to server.
        try
        {
            HttpResponseMessage response = await _userLoginApi.Login(new UserRequest(username, password));

            //TODO Add condition if server is unresponsive / not avalible
            if (response.StatusCode == HttpStatusCode.OK)
            {

                // Log stored cookies for debugging
                Uri baseUri = new Uri("http://localhost:5251");
                var cookies = _cookieContainer.GetCookies(baseUri);
                foreach (Cookie cookie in cookies)
                {
                    Console.WriteLine($"Cookie: {cookie.Name} = {cookie.Value}");
                }



                await Shell.Current.GoToAsync(nameof(DashboardPage));
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Application.Current.Windows[0].Page.DisplayAlert("Unauthorized", "Invalid login details", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    [RelayCommand]
    public async Task NewUserPrompt()
    {
        //Display popup for user to create login details.
        await _popupService.ShowPopupAsync<NewUserPopupViewModel>();
    }



}

 
    