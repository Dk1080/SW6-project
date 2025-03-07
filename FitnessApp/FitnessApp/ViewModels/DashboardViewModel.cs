using Avalonia.Controls;
namespace FitnessApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public DashboardViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    } 

    [RelayCommand]
    public void GoToChatBot()
    {
        _mainViewModel.NavigateTo(new ChatBotViewModel(_mainViewModel));
    }
    
    
}