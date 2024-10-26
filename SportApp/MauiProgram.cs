using Microsoft.Extensions.Logging;
using SportApp.Services;
using SportApp.ViewModels;
using SportApp.Views;
using CommunityToolkit.Maui;
namespace SportApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
        builder
			.UseMauiCommunityToolkit()
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        builder.Services.AddSingleton<FindEventsViewModel>();
		builder.Services.AddSingleton<FindEvent>();
		builder.Services.AddTransient<ViewEventViewModel>();
		builder.Services.AddTransient<ViewEvent>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoginView>();
        builder.Services.AddSingleton<LoginDemoWebAPIProxy>();
		builder.Services.AddTransient<CrateEventViewModel>();
		builder.Services.AddTransient<CrateEvent>();
        builder.Services.AddTransient<SignUpViewModel>();
        builder.Services.AddTransient<SignUpPage>();
		builder.Services.AddTransient<LogoutViewModel>();
		builder.Services.AddTransient<Logout>();
		builder.Services.AddTransient<UserDetailsViewModel>();
		builder.Services.AddTransient<UserDetailsPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
