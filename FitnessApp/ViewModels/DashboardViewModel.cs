using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp.Models.Api_DTOs;
using FitnessApp.Services.Apis;
using FitnessApp.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IDashboardApi _dashboardApi;
        
        public DashboardViewModel(IDashboardApi dashboardApi)
        {
            _dashboardApi = dashboardApi;
            LoadChartDataAsync(); // få data når man kommer ind på dashboard
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
    }
}
