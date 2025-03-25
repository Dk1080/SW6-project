using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FitnesApp.ViewModels;
using FitnesApp.Views;
using FitnessApp.ViewModels;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace FitnesApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseSkiaSharp()
            .UseLiveCharts()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();

		builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<DashboardViewModel>();

		builder.Services.AddTransient<ChatBotPage>();
		builder.Services.AddSingleton<ChatBotViewModel>();

        builder.Services.AddTransient<DebugPage>();
        builder.Services.AddSingleton<DebugViewModel>();




#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
