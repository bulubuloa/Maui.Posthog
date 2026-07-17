using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Maui.Posthog.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.AddPostHog(options =>
			{
				options.ApiKey = "phc_ovkjPAdo6zitCFWxoQy7a4cYeCfPMoHxYXoTescST83T";
				options.Host = "https://eu.i.posthog.com";
				options.CaptureScreenViews = true;
				options.SessionReplay = true;
				options.Autocapture = true;
				options.Debug = true;   // log SDK activity to the debug console
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// IPostHog is a lazy singleton — resolve it once so the native SDK actually initializes.
		var posthog = app.Services.GetRequiredService<IPostHog>();
		posthog.Capture("app_started");
		posthog.Flush();

		return app;
	}
}
