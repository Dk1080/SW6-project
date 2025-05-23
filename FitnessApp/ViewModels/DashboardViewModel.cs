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
#if ANDROID || WINDOWS|| IOS || MACCATALYST
using FitnessApp.PlatformsImplementations;
#endif
using FitnessApp.Services;
using DTOs;
using FitnessApp.Services.Apis;
using FitnessApi.Models.Api_DTOs;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;

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
                var chartTask = _dashboardApi.GetChartData();
                var preferenceTask = _dashboardApi.GetUserPreferences();

                await Task.WhenAll(chartTask, preferenceTask);

                var overviewData = chartTask.Result;
                var preferenceData = preferenceTask.Result;

                if (overviewData == null || !overviewData.Any())
                {
                    Console.WriteLine("No workout data received.");
                    return;
                }
                Debug.WriteLine($"[DashboardViewModel] Raw data ({overviewData.Count} items): [{string.Join(", ", overviewData.Select(w => $"Date={w.Date},Value={w.Value}"))}]");


                string interval = "weekly";
                DateTime todayDate = DateTime.UtcNow.Date;
                if (preferenceData.Goals.Any()) 
                {
                    var stepsGoal = preferenceData.Goals.FirstOrDefault();
                    if (stepsGoal != null)
                    {
                        interval = stepsGoal.Interval ?? "weekly";
                    }
                    Debug.WriteLine($"Interval is : {interval}");
                }


                DateTime cutoffDate;
                switch (interval.ToLower())
                {
                    case "weekly":
                        cutoffDate = DateTime.UtcNow.Date.AddDays(-7);
                        break;
                    case "biweekly":
                        cutoffDate = DateTime.UtcNow.Date.AddDays(-14);
                        break;
                    case "monthly":
                        cutoffDate = DateTime.UtcNow.Date.AddDays(-30);
                        break;
                    default:
                        cutoffDate = DateTime.UtcNow.Date.AddDays(-7); 
                        break;
                }

                Debug.WriteLine($"[DashboardViewModel] Filter parameters: cutoffDate={cutoffDate:yyyy-MM-dd}, startDate={todayDate:yyyy-MM-dd}, today={DateTime.UtcNow.Date:yyyy-MM-dd}");

                var filteredData = new List<ChartDataDTO>();
                foreach (var w in overviewData)
                {
                    try
                    {
                        var dataDate = DateTime.Parse(w.Date).Date;
                        bool inRange = dataDate <= todayDate && dataDate >= cutoffDate;
                        Debug.WriteLine($"[DashboardViewModel] Evaluating: Date={w.Date}, Value={w.Value}, Parsed={dataDate:yyyy-MM-dd}, InRange={inRange} " +
                                        $"(todayDate={dataDate <= todayDate}, cutoffDate={dataDate >= cutoffDate})");
                        if (inRange)
                        {
                            filteredData.Add(w);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[DashboardViewModel] Invalid date format: Date={w.Date}, Error={ex.Message}");
                    }
                }
                filteredData = filteredData.OrderBy(w => DateTime.Parse(w.Date)).ToList();

                Debug.WriteLine($"[DashboardViewModel] Filtered data ({filteredData.Count} items): [{string.Join(", ", filteredData.Select(w => $"Date={w.Date},Value={w.Value}"))}]");



                List<double> values;
                List<string> labels;
                
                values = filteredData.Select(w => w.Value).ToList();
                labels = filteredData.Select(w => DateTime.Parse(w.Date).ToString("MM-dd")).ToList();
                

                // Lav graf med data 
                OverviewDataGraph = new ISeries[]
                {
                    new ColumnSeries<Double>
                    {
                        Values = values
                    }
                };

                // X-akse values 
                XAxesOverviewGraph[0].Labels = labels;

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
                    var goal = preferenceData.Goals.FirstOrDefault();
                    if (goal != null && goal.Value > 0)
                    {
                        goalValue = goal.Value;
                    }
                    Debug.WriteLine($"Using data goal: {goalValue} value");
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
                    "Circle" => new ISeries[]
                    {
                        new PieSeries<int>
                        {
                            Values = new[] { percentage },
                            Name = "Today’s Progress",
                            InnerRadius = 50, // Doughnut hole
                            Fill = new SolidColorPaint(new SkiaSharp.SKColor(0, 255, 0)), // grøn
                            DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:F1}% / 100%",
                            DataLabelsSize = 15,
                            DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#000000")),
                            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.ChartCenter 
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
                            DataLabelsSize = 18,
                            DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top, 
                            DataLabelsPadding = new LiveChartsCore.Drawing.Padding(0, -10) 

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

                if(preferenceData.ChartPreference == "Circle" || preferenceData.ChartPreference == "Column") 
                {
                    // X-akse value
                    XAxesChartSeries[0].Labels = new List<string> { today.ToString("MM-dd") };
                }
                

                // Determine dynamic max limit
                int dynamicMax = Math.Max(100, percentage + 25); // Add a buffer if needed
                Console.WriteLine($"dynamicMax: {dynamicMax}");

                var labels = new List<string>();
                for (int i = 0; i <= dynamicMax + 25; i += 25)
                {
                    labels.Add($"{i}%");
                }
                Console.WriteLine($"Labels: {string.Join(", ", labels)}");

                YAxesChartSeries = new[]
                {
                    new Axis
                    {
                        MinLimit = 0,
                        MaxLimit = dynamicMax,
                        Labels = labels,
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
