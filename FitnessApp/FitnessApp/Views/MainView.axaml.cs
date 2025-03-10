using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FitnessApp.ViewModels;

namespace FitnessApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel(); 

    }
    
}