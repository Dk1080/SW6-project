using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnesApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnesApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [RelayCommand]
        async Task GoToChatBot()
        {
            await Shell.Current.GoToAsync(nameof(ChatBotPage));
        }


        [RelayCommand]
        async Task GoToDebug()
        {
            await Shell.Current.GoToAsync(nameof(DebugPage));
        }


    }
}
