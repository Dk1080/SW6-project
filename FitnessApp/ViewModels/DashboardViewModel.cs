using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessApp.Services.Apis;
using FitnessApp.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;

using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.PlatformsImplementations;
using FitnessApp.Services;
using DTOs;
using FitnessApp.Services.Apis;
using FitnessApi.Models.Api_DTOs;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

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
            LoadOverviewDataAsync();
            this.healthService = healthService;
            _healthApi = healthApi;
            SendDataToServer();
        }

        [ObservableProperty]
        private IEnumerable<ISeries> chartSeries;

        [ObservableProperty]
        private IEnumerable<ISeries> overviewDataGraph;

        [ObservableProperty]
        private Axis[] xAxesChartSeries = new[]
        {
            new Axis
            {
                Labels = new List<string>(),
                LabelsRotation = 15
            }
        };

        [ObservableProperty]
        private Axis[] yAxesChartSeries = new[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 100,
                Labels = new[] { "0%", "25%", "50%", "75%", "100%" }.ToList()
            }
        };

        [ObservableProperty]
        private Axis[] xAxesOverviewGraph = new[]
        {
            new Axis
            {
                Labels = new List<string>(),
                LabelsRotation = 15
            }
        };


        private async void LoadOverviewDataAsync()
        {
            try
            {
                // Fetch dataen til graferne fra APIen
                var overviewData = await _dashboardApi.GetChartData(); 

                if (overviewData == null || !overviewData.Any())
                {
                    Console.WriteLine("No workout data received.");
                    return;
                }

                // Lav graf med data 
                OverviewDataGraph = new ISeries[]
                {
                    new ColumnSeries<int>
                    {
                        Values = overviewData.Select(w => w.Value).ToList()
                    }
                };

                // X-akse values 
                XAxesOverviewGraph[0].Labels = overviewData.Select(w => DateTime.Parse(w.Date).ToString("MM-dd")).ToList();

                // Opdater UI
                OnPropertyChanged(nameof(OverviewDataGraph));
                OnPropertyChanged(nameof(XAxesOverviewGraph));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chart data: {ex.Message}");
            }
        }

        private async void LoadChartDataAsync()
        {
            try
            {
                // Fetch dataen til graferne fra APIen
                var chartTask = _dashboardApi.GetChartData();
                var preferenceTask = _dashboardApi.GetUserPreferences();

                await Task.WhenAll(chartTask, preferenceTask);

                var chartData = chartTask.Result;
                var preferenceData = preferenceTask.Result ?? new UserPreferencesDTO();

                double goalValue = 0; // default hvis der ikke kommer en
                if (preferenceData.Goals.Any())
                {
                    var stepsGoal = preferenceData.Goals.FirstOrDefault(g => g.GoalType == "steps");
                    if (stepsGoal != null && stepsGoal.Value > 0)
                    {
                        goalValue = stepsGoal.Value;
                    }
                    Debug.WriteLine($"Using steps goal: {goalValue} steps");
                }

                // Filter så man får dataen for i dag
                var today = DateTime.Today;
                var todayData = chartData.FirstOrDefault(w => DateTime.Parse(w.Date).Date == today);

                // Udregn % for i dag
                int percentage;
                if (todayData != null)
                {
                    percentage = (int)Math.Round((todayData.Value / goalValue) * 100);
                }
                else
                {
                    percentage = 0; // 0 hvis der ikke er data
                }

                // Lav graf med data 
                ChartSeries = preferenceData.ChartPreference switch
                {
                    "Halfcircle" => new ISeries[]
                    {
                        new PieSeries<int>
                        {
                            Values = new[] { percentage },
                            Name = "Today’s Progress",
                            InnerRadius = 50, // Doughnut hole
                            Fill = new SolidColorPaint(new SkiaSharp.SKColor(0, 255, 0)), // grøn       
                            DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#000000")),
                        },
                        new PieSeries<int>
                        {
                            Values = new[] { 100 - percentage },
                            Name = "Remaining",
                            InnerRadius = 50,
                            Fill = new SolidColorPaint(new SkiaSharp.SKColor(200, 200, 200, 100)), // grå, transparent
                            DataLabelsFormatter = point => ""
                        }
                    },
                    "Column" => new ISeries[]
                    {
                        new ColumnSeries<int>
                        {
                            Values = new[] { percentage },
                            Name = "Today’s Progress (% of Goal)",
                            Fill = new SolidColorPaint(SKColor.Parse("#00FF00")), // Grøn
                            DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:F1}% / 100%", 
                            DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#000000")),
                            DataLabelsSize = 20

                        }
                    },

                    _ => new ISeries[] // Default to Column
                    {
                        new ColumnSeries<int>
                        {
                            Values = new[] { percentage },
                            Name = "Daily Progress (% of Goal)"
                        }
                    }
                };

                // X-akse value
                XAxesChartSeries[0].Labels = new List<string> { today.ToString("MM-dd") };

                YAxesChartSeries = new[]
                {
                    new Axis
                    {
                        MinLimit = 0,
                        MaxLimit = 100,
                        Labels = new[] { "0%", "25%", "50%", "75%", "100%" }.ToList(),
                        LabelsPaint = new SolidColorPaint(SKColor.Parse("#000000")),
                        SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)) { StrokeThickness = 1 }
                    }
                };

                // Opdater UI
                OnPropertyChanged(nameof(ChartSeries));
                OnPropertyChanged(nameof(XAxesChartSeries));
                OnPropertyChanged(nameof(YAxesChartSeries));

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading chart data: {ex.Message}");
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
