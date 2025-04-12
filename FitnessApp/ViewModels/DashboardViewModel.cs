using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp.Services.Apis;
using FitnessApp.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if ANDROID || WINDOWS|| IOS || MACCATALYST
using FitnessApp.PlatformsImplementations;
#endif
using FitnessApp.Services;
using DTOs;
using FitnessApp.Services.Apis;

namespace FitnessApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IDashboardApi _dashboardApi;
        IHealthService healthService;
        IHealthApi _healthApi;

        public DashboardViewModel(IHealthService healthService, IHealthApi healthApi ,IDashboardApi dashboardApi)
        {
            _dashboardApi = dashboardApi;
            LoadChartDataAsync(); // få data når man kommer ind på dashboard
            this.healthService = healthService;
            _healthApi = healthApi;
            SendDataToServer();
        }

        [ObservableProperty]
        private IEnumerable<ISeries> chartSeries;

        [ObservableProperty]
        private Axis[] xAxes = new[]
        {
            new Axis
            {
                Labels = new List<string>(),
                LabelsRotation = 15
            }
        };

        private async void LoadChartDataAsync()
        {
            try
            {
                // Fetch dataen til graferne fra APIen
                var ChartData = await _dashboardApi.GetChartData();

                if (ChartData == null || !ChartData.Any())
                {
                    Console.WriteLine("No workout data received.");
                    return;
                }

                // Lav graf med data 
                ChartSeries = new ISeries[]
                {
                    new ColumnSeries<int>
                    {
                        Values = ChartData.Select(w => w.Value).ToList()
                    }
                };

                // X-akse values 
                xAxes[0].Labels = ChartData.Select(w => DateTime.Parse(w.Date).ToString("MM-dd")).ToList();

                // Opdater UI
                OnPropertyChanged(nameof(ChartSeries));
                OnPropertyChanged(nameof(xAxes));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chart data: {ex.Message}");
            }
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
