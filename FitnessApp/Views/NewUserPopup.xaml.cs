using CommunityToolkit.Maui.Views;
using FitnessApp.ViewModels;

namespace FitnessApp;
public partial class NewUserPopup : Popup
{
    public NewUserPopup()
    {
        InitializeComponent();
    }

    public NewUserPopup(NewUserPopupViewModel vm) : this()
    {
        BindingContext = vm;
    }
}

