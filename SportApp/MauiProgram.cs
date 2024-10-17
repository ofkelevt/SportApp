using Microsoft.Extensions.Logging;
using SportApp.Services;
using SportApp.ViewModels;
using SportApp.Views;

namespace SportApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddSingleton<FindEventsViewModel>();
		builder.Services.AddSingleton<FindEvent>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
