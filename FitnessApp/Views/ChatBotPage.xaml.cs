using FitnessApp.ViewModels;

namespace FitnessApp;

public partial class ChatBotPage : ContentPage
{
	public ChatBotPage(ChatBotViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}