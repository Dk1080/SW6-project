using FitnesApp.ViewModels;

namespace FitnesApp;

public partial class ChatBotPage : ContentPage
{
	public ChatBotPage(ChatBotViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}