using FitnessApp.ViewModels;

namespace FitnessApp;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

    }
}