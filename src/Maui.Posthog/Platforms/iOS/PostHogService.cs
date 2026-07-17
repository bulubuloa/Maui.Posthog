using Foundation;
using PostHogBinding;

namespace Maui.Posthog;

internal sealed class PostHogService : IPostHog
{
    private readonly PostHogSDK _client;

    public PostHogService(PostHogOptions options)
    {
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

        _client = PostHogSDK.Shared;
        _client.Setup(config);
    }

    public string DistinctId => _client.GetDistinctId();

    public void Capture(string eventName, IDictionary<string, object>? properties = null)
        => _client.Capture(eventName, ToNs(properties));

    public void Identify(string distinctId, IDictionary<string, object>? userProperties = null, IDictionary<string, object>? userPropertiesSetOnce = null)
        => _client.Identify(distinctId, ToNs(userProperties), ToNs(userPropertiesSetOnce));

    public void Screen(string screenName, IDictionary<string, object>? properties = null)
        => _client.Screen(screenName, ToNs(properties));

    public void Alias(string alias) => _client.Alias(alias);

    public void Group(string type, string key, IDictionary<string, object>? properties = null)
        => _client.Group(type, key, ToNs(properties));

    public bool IsFeatureEnabled(string key) => _client.IsFeatureEnabled(key);

    public object? GetFeatureFlag(string key) => Unwrap(_client.GetFeatureFlag(key));

    public object? GetFeatureFlagPayload(string key) => Unwrap(_client.GetFeatureFlagPayload(key));

    public void ReloadFeatureFlags() => _client.ReloadFeatureFlags();

    public void Register(string key, object value)
        => _client.Register(NSDictionary.FromObjectAndKey(NSObject.FromObject(value), new NSString(key)));

    public void Unregister(string key) => _client.Unregister(key);

    public void Reset() => _client.Reset();

    public void Flush() => _client.Flush();

    public void OptIn() => _client.OptIn();

    public void OptOut() => _client.OptOut();

    public void StartSessionReplay() => _client.StartSessionRecording(true);

    public void StopSessionReplay() => _client.StopSessionRecording();

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
