using Com.Posthog;
using Com.Posthog.Android;
using AndroidReplayConfig = Com.Posthog.Android.Replay.PostHogSessionReplayConfig;
using JObject = Java.Lang.Object;

namespace Maui.Posthog;

internal sealed class PostHogService : IPostHog
{
    private readonly IPostHogInterface _client;

    public PostHogService(PostHogOptions options)
    {
        var config = new PostHogAndroidConfig(
            options.ApiKey,
            options.Host,
            options.CaptureApplicationLifecycleEvents,
            captureDeepLinks: true,
            captureScreenViews: options.CaptureScreenViews)
        {
            Debug = options.Debug,
            FlushAt = options.FlushAt,
            SessionReplay = options.SessionReplay,
        };

        if (options.SessionReplay)
        {
            AndroidReplayConfig replay = config.SessionReplayConfig;
            replay.MaskAllTextInputs = options.Replay.MaskAllTextInputs;
            replay.MaskAllImages = options.Replay.MaskAllImages;
            replay.Screenshot = options.Replay.ScreenshotMode;
        }

        // with() runs the full setup pipeline (integrations, replay) and returns the instance.
        _client = PostHogAndroid.Shared.With(global::Android.App.Application.Context!, config);
    }

    public string DistinctId => _client.DistinctId();

    public void Capture(string eventName, IDictionary<string, object>? properties = null)
        => _client.Capture(eventName, null, ToJava(properties), null, null, null);

    public void Identify(string distinctId, IDictionary<string, object>? userProperties = null, IDictionary<string, object>? userPropertiesSetOnce = null)
        => _client.Identify(distinctId, ToJava(userProperties), ToJava(userPropertiesSetOnce));

    public void Screen(string screenName, IDictionary<string, object>? properties = null)
        => _client.Screen(screenName, ToJava(properties));

    public void Alias(string alias) => _client.Alias(alias);

    public void Group(string type, string key, IDictionary<string, object>? properties = null)
        => _client.Group(type, key, ToJava(properties));

    public bool IsFeatureEnabled(string key) => _client.IsFeatureEnabled(key, false);

    public object? GetFeatureFlag(string key) => Unwrap(_client.GetFeatureFlag(key, null));

    public object? GetFeatureFlagPayload(string key) => Unwrap(_client.GetFeatureFlagPayload(key, null));

    public void ReloadFeatureFlags() => _client.ReloadFeatureFlags(null);

    public void Register(string key, object value) => _client.Register(key, ToJava(value));

    public void Unregister(string key) => _client.Unregister(key);

    public void Reset() => _client.Reset();

    public void Flush() => _client.Flush();

    public void OptIn() => _client.OptIn();

    public void OptOut() => _client.OptOut();

    public void StartSessionReplay() => _client.StartSessionReplay(true);

    public void StopSessionReplay() => _client.StopSessionReplay();

    private static IDictionary<string, JObject>? ToJava(IDictionary<string, object>? source)
    {
        if (source is null)
            return null;

        var result = new Dictionary<string, JObject>(source.Count);
        foreach (var kvp in source)
            result[kvp.Key] = ToJava(kvp.Value);
        return result;
    }

    private static JObject ToJava(object value) => value switch
    {
        null => null!,
        JObject jo => jo,
        string s => new Java.Lang.String(s),
        bool b => new Java.Lang.Boolean(b),
        int i => new Java.Lang.Integer(i),
        long l => new Java.Lang.Long(l),
        double d => new Java.Lang.Double(d),
        float f => new Java.Lang.Float(f),
        _ => new Java.Lang.String(value.ToString()),
    };

    private static object? Unwrap(JObject? value) => value switch
    {
        null => null,
        Java.Lang.Boolean b => (bool)b,
        Java.Lang.String s => s.ToString(),
        Java.Lang.Integer i => (int)i,
        Java.Lang.Long l => (long)l,
        Java.Lang.Double d => (double)d,
        _ => value.ToString(),
    };
}
