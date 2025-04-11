using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp.Helpers;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration;
using FitnessApp.Services;

#if ANDROID
using Testing = NewBindingAndroid.DotnetNewBinding;
#endif

namespace FitnessApp.ViewModels
{
    public partial class DebugViewModel : ObservableObject
    {

#if ANDROID
        [ObservableProperty]
        string labelText = "Hello, " + Testing.GetString("hello");
#endif

        IHealthService healthService;

        public DebugViewModel(IHealthService healthService)
        {
            this.healthService = healthService;
        }




         [ObservableProperty]
        int x = 0;


        [RelayCommand]
        public void Increment()
        {
            X++;
        }

        [RelayCommand]
        public async Task GetSteps()
        {
            var y = await healthService.GetSteps();
        }

        [RelayCommand]

        public async Task Insertsteps()
        {
            await healthService.DebugInsertSteps(200);
            await healthService.DebugInsertSteps(300);
        }




        [RelayCommand]
        async Task RequestPermissions()
        {
            await healthService.RequestPermission();
        }



    }


}
