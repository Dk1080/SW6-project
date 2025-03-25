using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnesApp.Views;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnesApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {

        public DashboardViewModel()
        {
            LoadChartData();
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
    }
}
