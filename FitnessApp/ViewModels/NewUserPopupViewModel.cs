using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp.Services.APIs;

namespace FitnessApp.ViewModels
{
    public partial class NewUserPopupViewModel : ObservableObject
    {

        private IUserLoginApi _userLoginApi;

        public NewUserPopupViewModel(IUserLoginApi userLoginApi)
        {
            _userLoginApi = userLoginApi;
        }


        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string responseMessage;


        [RelayCommand]
        public async Task CreateUser()
        {
            Console.WriteLine($"username: {Username} Password: {Password}");

            //Send API request to create user.
            var response = await _userLoginApi.AddUser(new DTOs.UserRequest(Username, Password));

            if(response.StatusCode == HttpStatusCode.OK)
            {
                ResponseMessage = "User has been created.";
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ResponseMessage = "Username is taken.";
            } else
            {
                ResponseMessage = "ERROR";
            }

        }


    }
}
