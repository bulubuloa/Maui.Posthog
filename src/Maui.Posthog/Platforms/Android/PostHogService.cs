using Com.Posthog;
using Com.Posthog.Android;
using AndroidReplayConfig = Com.Posthog.Android.Replay.PostHogSessionReplayConfig;
using JObject = Java.Lang.Object;

namespace Maui.Posthog;

/// <summary>Android implementation of <see cref="IPostHog"/>. All members are virtual; subclass to override behavior.</summary>
public class PostHogService : IPostHog
{
    /// <summary>The underlying native PostHog client, exposed for subclasses.</summary>
    protected IPostHogInterface Client { get; }

    public PostHogService(PostHogOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

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
        Client = PostHogAndroid.Shared.With(global::Android.App.Application.Context!, config);
    }

    public virtual string DistinctId => Client.DistinctId();

    public virtual void Capture(string eventName, IDictionary<string, object>? properties = null)
        => Client.Capture(eventName, null, ToJava(properties), null, null, null);

    public virtual void Identify(string distinctId, IDictionary<string, object>? userProperties = null, IDictionary<string, object>? userPropertiesSetOnce = null)
        => Client.Identify(distinctId, ToJava(userProperties), ToJava(userPropertiesSetOnce));

    public virtual void Screen(string screenName, IDictionary<string, object>? properties = null)
        => Client.Screen(screenName, ToJava(properties));

    public virtual void Alias(string alias) => Client.Alias(alias);

    public virtual void Group(string type, string key, IDictionary<string, object>? properties = null)
        => Client.Group(type, key, ToJava(properties));

    public virtual bool IsFeatureEnabled(string key) => Client.IsFeatureEnabled(key, false);

    public virtual object? GetFeatureFlag(string key) => Unwrap(Client.GetFeatureFlag(key, null));

    public virtual object? GetFeatureFlagPayload(string key) => Unwrap(Client.GetFeatureFlagPayload(key, null));

    public virtual void ReloadFeatureFlags() => Client.ReloadFeatureFlags(null);

    public virtual void Register(string key, object value) => Client.Register(key, ToJava(value));

    public virtual void Unregister(string key) => Client.Unregister(key);

    public virtual void Reset() => Client.Reset();

    public virtual void Flush() => Client.Flush();

    public virtual void OptIn() => Client.OptIn();

    public virtual void OptOut() => Client.OptOut();

    public virtual void StartSessionReplay() => Client.StartSessionReplay(true);

    public virtual void StopSessionReplay() => Client.StopSessionReplay();

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
