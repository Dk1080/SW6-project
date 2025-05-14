using CommunityToolkit.Mvvm.Messaging;
using FitnessApp.ViewModels;
using Microsoft.Maui.Controls;

namespace FitnessApp;

public partial class ChatBotPage : ContentPage
{
	public ChatBotPage(ChatBotViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        // Subscribe to ScrollToBottomMessage
        WeakReferenceMessenger.Default.Register<ScrollToBottomMessage>(this, (recipient, message) =>
        {
            var collectionView = this.FindByName<CollectionView>("ChatCollectionView");
            if (collectionView?.ItemsSource != null)
            {
                var items = collectionView.ItemsSource.Cast<object>().ToList();
                if (items.Any())
                {
                    collectionView.ScrollTo(items.Last(), position: ScrollToPosition.End, animate: true);
                }
            }
        });

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Unregister to prevent memory leaks
        WeakReferenceMessenger.Default.Unregister<ScrollToBottomMessage>(this);
    }
}