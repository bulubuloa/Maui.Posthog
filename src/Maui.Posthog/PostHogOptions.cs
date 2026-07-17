namespace Maui.Posthog;

/// <summary>Startup configuration for the PostHog SDK.</summary>
public sealed class PostHogOptions
{
    /// <summary>Project API key (write-only key) from PostHog project settings. Required.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Ingestion host. Defaults to PostHog US Cloud. Use https://eu.i.posthog.com for EU.</summary>
    public string Host { get; set; } = "https://us.i.posthog.com";

    /// <summary>Capture app opened/backgrounded/updated lifecycle events. Default true.</summary>
    public bool CaptureApplicationLifecycleEvents { get; set; } = true;

    /// <summary>Automatically capture screen views. Default true.</summary>
    public bool CaptureScreenViews { get; set; } = true;

    /// <summary>Automatically capture UI element interactions (iOS autocapture). Default false.</summary>
    public bool Autocapture { get; set; }

    /// <summary>Enable session replay recording. Default false.</summary>
    public bool SessionReplay { get; set; }

    /// <summary>Verbose SDK logging. Default false.</summary>
    public bool Debug { get; set; }

    /// <summary>Number of queued events that triggers an automatic flush. Default 20.</summary>
    public int FlushAt { get; set; } = 20;

    /// <summary>Session replay masking / privacy settings.</summary>
    public SessionReplayOptions Replay { get; set; } = new();
}

/// <summary>Session replay privacy controls (only used when <see cref="PostHogOptions.SessionReplay"/> is true).</summary>
public sealed class SessionReplayOptions
{
    /// <summary>Mask all text input fields. Default true.</summary>
    public bool MaskAllTextInputs { get; set; } = true;

    /// <summary>Mask all images. Default true.</summary>
    public bool MaskAllImages { get; set; } = true;

    /// <summary>Capture the screen as periodic screenshots (recommended for non-native UI). Default true.</summary>
    public bool ScreenshotMode { get; set; } = true;
}
