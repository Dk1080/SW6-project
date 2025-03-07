using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FitnessApp.ViewModels;

/* MainViewModel holder styr på vores app navigation lige nu så hvis i laver en side som skal navigeres til
 så gør det med _mainViewModel.NavigateTo lige som i fx DashboardViewModel fordi MainView.axaml har en ContentControl
 som gør at den korrekte view bliver indlæst når man fx trykker på en knap der skal tage en væk
 
 Husk at opdatere ViewLocator.cs med nye viewmodels hvis de skal bruges
 */
public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase? _currentPage;
    public MainViewModel()
    {
        // Start with MainViewModel as the default page
        CurrentPage = this;
    }
    
    [RelayCommand]
    public void GoToDashboard()
    {
        NavigateTo(new DashboardViewModel(this));
    }
    
    
    
    public void NavigateTo(ViewModelBase viewModel)
    {
            CurrentPage = viewModel;
    }
    
}