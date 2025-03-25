using FitnesApp.ViewModels;
using FitnesApp.Views;

namespace FitnesApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            Routing.RegisterRoute(nameof(ChatBotPage), typeof(ChatBotPage));
            Routing.RegisterRoute(nameof(DebugPage), typeof(DebugPage));
        }
    }
}
