using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FitnessApp.PlatformsImplementations;
using FitnessApp.Services;
using FitnessApp.Services.Apis;
using FitnessApp.Services.APIs;
using FitnessApp.ViewModels;
using FitnessApp.Views;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using SkiaSharp.Views.Maui.Controls.Hosting;
using System.Net;

namespace FitnessApp;

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

        // Create a shared CookieContainer
        var cookieContainer = new CookieContainer();
        builder.Services.AddSingleton(cookieContainer);

        var handler = new HttpClientHandler { CookieContainer = cookieContainer };

        //Add refit services to send Http requests
        builder.Services.AddRefitClient<IUserLoginApi>()
#if ANDROID
			.ConfigureHttpClient(c => c.BaseAddress = new Uri("http://10.0.2.2:5251"))
#else
			.ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5251"))
#endif
			.ConfigurePrimaryHttpMessageHandler(() => handler);

        builder.Services.AddRefitClient<IChatApi>()
#if ANDROID
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://10.0.2.2:5251"))
#else
			.ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5251"))
#endif
    .ConfigurePrimaryHttpMessageHandler(() => handler);




        builder.Services.AddSingleton<IHealthService>((e)=> new HealthService());


        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

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
