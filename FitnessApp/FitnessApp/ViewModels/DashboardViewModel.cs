using Avalonia.Controls;
namespace FitnessApp.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public DashboardViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
    
}