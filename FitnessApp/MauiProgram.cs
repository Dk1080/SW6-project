using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
#if ANDROID || WINDOWS|| IOS || MACCATALYST
using FitnessApp.PlatformsImplementations;
#endif
using FitnessApp.Services;
using FitnessApp.Converters;
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

        var handler = new HttpClientHandler { CookieContainer = cookieContainer, UseCookies = true };
        builder.Services.AddSingleton(handler);

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

        builder.Services.AddRefitClient<IHealthApi>()
#if ANDROID
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://10.0.2.2:5251"))
#else
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5251"))
#endif
            .ConfigurePrimaryHttpMessageHandler(() => handler);

        builder.Services.AddRefitClient<IDashboardApi>()
#if ANDROID
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://10.0.2.2:5251"))
#else
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5251"))
#endif
            .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<HttpClientHandler>());



        //Needs this conditial for unit testing.
#if ANDROID || WINDOWS|| IOS || MACCATALYST
    builder.Services.AddSingleton<IHealthService>((e)=> new HealthService());
#endif


        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddSingleton<DashboardPage>();
        builder.Services.AddSingleton<DashboardViewModel>();

		builder.Services.AddTransient<ChatBotPage>();
		builder.Services.AddSingleton<ChatBotViewModel>();

        builder.Services.AddTransient<DebugPage>();
        builder.Services.AddSingleton<DebugViewModel>();

        builder.Services.AddSingleton<IsPieSeriesConverter>();
        builder.Services.AddSingleton<IsNotPieSeriesConverter>();



#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
