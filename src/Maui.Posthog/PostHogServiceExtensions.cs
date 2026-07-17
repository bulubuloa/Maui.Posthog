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
        => builder.AddPostHog<PostHogService>(configure);

    /// <summary>
    /// Register PostHog analytics using a custom <see cref="PostHogService"/> subclass as the <see cref="IPostHog"/>
    /// implementation. The subclass must expose a public constructor accepting <see cref="PostHogOptions"/>.
    /// </summary>
    /// <typeparam name="TPostHog">A <see cref="PostHogService"/> subclass overriding one or more members.</typeparam>
    public static MauiAppBuilder AddPostHog<TPostHog>(this MauiAppBuilder builder, Action<PostHogOptions> configure)
        where TPostHog : PostHogService
    {
        var options = PostHogOptions.Create(configure);
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IPostHog, TPostHog>();
        return builder;
    }
}
