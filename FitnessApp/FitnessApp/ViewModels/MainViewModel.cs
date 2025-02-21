using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isPaneOpen = true;
    [ObservableProperty] private ViewModelBase _currentPage = new HomePageViewModel();

    [RelayCommand]
    public void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
    
}