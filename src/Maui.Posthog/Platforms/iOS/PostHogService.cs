using Foundation;
using PostHogBinding;

namespace Maui.Posthog;

/// <summary>iOS implementation of <see cref="IPostHog"/>. All members are virtual; subclass to override behavior.</summary>
public class PostHogService : IPostHog
{
    /// <summary>The underlying native PostHog SDK instance, exposed for subclasses.</summary>
    protected PostHogSDK Client { get; }

    public PostHogService(PostHogOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var config = new PostHogConfig(options.ApiKey, options.Host)
        {
            Debug = options.Debug,
            FlushAt = options.FlushAt,
            CaptureApplicationLifecycleEvents = options.CaptureApplicationLifecycleEvents,
            CaptureScreenViews = options.CaptureScreenViews,
            CaptureElementInteractions = options.Autocapture,
            SessionReplay = options.SessionReplay,
        };

        if (options.SessionReplay)
        {
            PostHogSessionReplayConfig replay = config.SessionReplayConfig;
            replay.MaskAllTextInputs = options.Replay.MaskAllTextInputs;
            replay.MaskAllImages = options.Replay.MaskAllImages;
            replay.ScreenshotMode = options.Replay.ScreenshotMode;
        }

        Client = PostHogSDK.Shared;
        Client.Setup(config);
    }

    public virtual string DistinctId => Client.GetDistinctId();

    public virtual void Capture(string eventName, IDictionary<string, object>? properties = null)
        => Client.Capture(eventName, ToNs(properties));

    public virtual void Identify(string distinctId, IDictionary<string, object>? userProperties = null, IDictionary<string, object>? userPropertiesSetOnce = null)
        => Client.Identify(distinctId, ToNs(userProperties), ToNs(userPropertiesSetOnce));

    public virtual void Screen(string screenName, IDictionary<string, object>? properties = null)
        => Client.Screen(screenName, ToNs(properties));

    public virtual void Alias(string alias) => Client.Alias(alias);

    public virtual void Group(string type, string key, IDictionary<string, object>? properties = null)
        => Client.Group(type, key, ToNs(properties));

    public virtual bool IsFeatureEnabled(string key) => Client.IsFeatureEnabled(key);

    public virtual object? GetFeatureFlag(string key) => Unwrap(Client.GetFeatureFlag(key));

    public virtual object? GetFeatureFlagPayload(string key) => Unwrap(Client.GetFeatureFlagPayload(key));

    public virtual void ReloadFeatureFlags() => Client.ReloadFeatureFlags();

    public virtual void Register(string key, object value)
        => Client.Register(NSDictionary.FromObjectAndKey(NSObject.FromObject(value), new NSString(key)));

    public virtual void Unregister(string key) => Client.Unregister(key);

    public virtual void Reset() => Client.Reset();

    public virtual void Flush() => Client.Flush();

    public virtual void OptIn() => Client.OptIn();

    public virtual void OptOut() => Client.OptOut();

    public virtual void StartSessionReplay() => Client.StartSessionRecording(true);

    public virtual void StopSessionReplay() => Client.StopSessionRecording();

    private static NSDictionary? ToNs(IDictionary<string, object>? source)
    {
        if (source is null || source.Count == 0)
            return null;

        var keys = new NSString[source.Count];
        var values = new NSObject[source.Count];
        var i = 0;
        foreach (var kvp in source)
        {
            keys[i] = new NSString(kvp.Key);
            values[i] = NSObject.FromObject(kvp.Value) ?? NSNull.Null;
            i++;
        }
        return NSDictionary.FromObjectsAndKeys(values, keys);
    }

    private static object? Unwrap(NSObject? value) => value switch
    {
        null => null,
        NSNumber n => n,
        NSString s => s.ToString(),
        _ => value,
    };
}
