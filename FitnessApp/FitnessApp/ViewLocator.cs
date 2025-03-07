using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FitnessApp.ViewModels;
using FitnessApp.Views;

namespace FitnessApp;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        return data switch
        {
            // Forbind ViewModel med View
            MainViewModel => new MainView(),          // MainViewModel -> MainView
            DashboardViewModel => new DashboardView(), // DashboardViewModel -> DashboardView
            ChatBotViewModel => new ChatBotView(), // ChatBotViewModel -> ChatBotView
            _ => new TextBlock { Text = "View not found" }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}