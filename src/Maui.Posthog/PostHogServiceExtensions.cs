using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace Maui.Posthog;

/// <summary>DI registration for PostHog.</summary>
public static class PostHogServiceExtensions
{
    /// <summary>
    /// Register PostHog analytics. Initializes the native SDK on first resolution of <see cref="IPostHog"/>.
    /// </summary>
    public static MauiAppBuilder AddPostHog(this MauiAppBuilder builder, Action<PostHogOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new PostHogOptions();
        configure(options);

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException("PostHogOptions.ApiKey must be set.", nameof(configure));

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IPostHog, PostHogService>();
        return builder;
    }
}
