using FitnesApp.ViewModels;

namespace FitnesApp;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

    }
}