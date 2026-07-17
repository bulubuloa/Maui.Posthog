namespace Maui.Posthog;

/// <summary>Cross-platform PostHog analytics surface. Resolve from DI after <c>AddPostHog</c>.</summary>
public interface IPostHog
{
    /// <summary>Capture a custom event with optional properties.</summary>
    void Capture(string eventName, IDictionary<string, object>? properties = null);

    /// <summary>Associate the current user with a distinct id and optional person properties.</summary>
    void Identify(string distinctId, IDictionary<string, object>? userProperties = null, IDictionary<string, object>? userPropertiesSetOnce = null);

    /// <summary>Capture a screen view.</summary>
    void Screen(string screenName, IDictionary<string, object>? properties = null);

    /// <summary>Link an anonymous identity to another distinct id.</summary>
    void Alias(string alias);

    /// <summary>Associate the user with a group (e.g. company).</summary>
    void Group(string type, string key, IDictionary<string, object>? properties = null);

    /// <summary>True when a boolean feature flag is enabled.</summary>
    bool IsFeatureEnabled(string key);

    /// <summary>Feature flag value (bool for boolean flags, string for multivariate), or null.</summary>
    object? GetFeatureFlag(string key);

    /// <summary>Feature flag JSON payload, or null.</summary>
    object? GetFeatureFlagPayload(string key);

    /// <summary>Force a refresh of feature flags from the server.</summary>
    void ReloadFeatureFlags();

    /// <summary>Register a super property sent with every subsequent event.</summary>
    void Register(string key, object value);

    /// <summary>Remove a previously registered super property.</summary>
    void Unregister(string key);

    /// <summary>The current distinct id.</summary>
    string DistinctId { get; }

    /// <summary>Reset the stored identity (call on logout).</summary>
    void Reset();

    /// <summary>Flush queued events immediately.</summary>
    void Flush();

    /// <summary>Opt the user in to capture.</summary>
    void OptIn();

    /// <summary>Opt the user out of capture.</summary>
    void OptOut();

    /// <summary>Start session replay recording (no-op if not enabled in options).</summary>
    void StartSessionReplay();

    /// <summary>Stop session replay recording.</summary>
    void StopSessionReplay();
}
