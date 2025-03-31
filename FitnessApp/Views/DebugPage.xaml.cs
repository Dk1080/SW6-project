using FitnessApp.ViewModels;

namespace FitnessApp.Views;

public partial class DebugPage : ContentPage
{
	public DebugPage(DebugViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}