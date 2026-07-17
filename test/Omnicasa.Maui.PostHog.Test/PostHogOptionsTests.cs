using Maui.Posthog;
using Xunit;

namespace Omnicasa.Maui.PostHog.Test;

/// <summary>
/// Unit tests for <see cref="PostHogOptions"/> defaults and validation.
/// </summary>
public class PostHogOptionsTests
{
    /// <summary>
    /// A freshly constructed instance exposes the documented default values.
    /// </summary>
    [Fact]
    public void Defaults_AreExpected()
    {
        var options = new PostHogOptions();

        Assert.Equal(string.Empty, options.ApiKey);
        Assert.Equal("https://us.i.posthog.com", options.Host);
        Assert.True(options.CaptureApplicationLifecycleEvents);
        Assert.True(options.CaptureScreenViews);
        Assert.False(options.Autocapture);
        Assert.False(options.SessionReplay);
        Assert.False(options.Debug);
        Assert.Equal(20, options.FlushAt);
        Assert.NotNull(options.Replay);
    }

    /// <summary>
    /// Session replay masking defaults to the most private configuration.
    /// </summary>
    [Fact]
    public void SessionReplayDefaults_AreMasked()
    {
        var replay = new SessionReplayOptions();

        Assert.True(replay.MaskAllTextInputs);
        Assert.True(replay.MaskAllImages);
        Assert.True(replay.ScreenshotMode);
    }

    /// <summary>
    /// A null configuration callback is rejected.
    /// </summary>
    [Fact]
    public void Create_NullConfigure_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => PostHogOptions.Create(null!));
    }

    /// <summary>
    /// A missing or blank API key is rejected.
    /// </summary>
    /// <param name="apiKey">The invalid API key value under test.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_BlankApiKey_Throws(string? apiKey)
    {
        Assert.Throws<ArgumentException>(() => PostHogOptions.Create(o => o.ApiKey = apiKey!));
    }

    /// <summary>
    /// A valid callback returns an instance carrying the configured values.
    /// </summary>
    [Fact]
    public void Create_ValidConfigure_AppliesValues()
    {
        var options = PostHogOptions.Create(o =>
        {
            o.ApiKey = "phc_key";
            o.Host = "https://eu.i.posthog.com";
            o.SessionReplay = true;
            o.Autocapture = true;
            o.FlushAt = 5;
        });

        Assert.Equal("phc_key", options.ApiKey);
        Assert.Equal("https://eu.i.posthog.com", options.Host);
        Assert.True(options.SessionReplay);
        Assert.True(options.Autocapture);
        Assert.Equal(5, options.FlushAt);
    }
}
