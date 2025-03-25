using FitnesApp.ViewModels;

namespace FitnesApp.Views;

public partial class DebugPage : ContentPage
{
	public DebugPage(DebugViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}