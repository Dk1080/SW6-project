using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    //[ObservableProperty] private bool _isPaneOpen = true;
    [ObservableProperty] private ViewModelBase? _currentPage;
    
    /*[RelayCommand]
    public void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }*/
    
    [RelayCommand]
    public void GoToDashboard()
    {
        NavigateTo(new DashboardViewModel(this));  // Navigate to DashboardViewModel
    }
    
    public void NavigateTo(ViewModelBase viewModel)
    {
            CurrentPage = viewModel;
    }
    
}