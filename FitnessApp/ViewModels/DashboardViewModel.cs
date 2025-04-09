using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp.Views;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.PlatformsImplementations;
using FitnessApp.Services;
using DTOs;
using FitnessApp.Services.Apis;

namespace FitnessApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {

        IHealthService healthService;
        IHealthApi _healthApi;

        public DashboardViewModel(IHealthService healthService, IHealthApi healthApi)
        {
            LoadChartData();
            this.healthService = healthService;
            _healthApi = healthApi;
            SendDataToServer();
        }

        [ObservableProperty]
        private IEnumerable<ISeries> chartSeries;

        private void LoadChartData()
        {
            ChartSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = new List<int> {10, 20, 30, 40, 50},
                    Name = "Workout Progress i guess"
                }
            };
        }

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



        async Task SendDataToServer()
        {
            //Check that we have permission
            var status = PermissionStatus.Unknown;
            while (status != PermissionStatus.Granted) {
                status = await healthService.RequestPermission();
                if (status == PermissionStatus.Denied)
                {
                    await Application.Current.Windows[0].Page.DisplayAlert("Permission not granted", "We need access to health data for the app to work", "OK");
                }
            }


            //Get step data TODO this should be reworked when we want more data so that we have an agregate method that calls the methods for each of the individual data methods.
            var healthList = await healthService.GetSteps();

            //Create new healthInfoDTO and send it to the server.
            var userHealthInfo = new HealthInfoDTO(healthList);

            //Send the data to the server.
            try
            {
                HttpResponseMessage response = await _healthApi.sendHealthData(userHealthInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


        }




    }
}
