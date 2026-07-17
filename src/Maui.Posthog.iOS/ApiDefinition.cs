using System;
using Foundation;
using ObjCRuntime;

namespace PostHogBinding;

// Swift @objc(PostHogConfig) -> clean ObjC runtime name.
[BaseType(typeof(NSObject), Name = "PostHogConfig")]
[DisableDefaultCtor]
interface PostHogConfig
{
    [Export("projectToken:")]
    NativeHandle Constructor(string projectToken);

    [Export("projectToken:host:")]
    NativeHandle Constructor(string projectToken, string host);

    [Export("host", ArgumentSemantic.Copy)]
    NSUrl Host { get; }

    [Export("projectToken")]
    string ProjectToken { get; }

    [Export("flushAt")]
    nint FlushAt { get; set; }

    [Export("maxQueueSize")]
    nint MaxQueueSize { get; set; }

    [Export("maxBatchSize")]
    nint MaxBatchSize { get; set; }

    [Export("flushIntervalSeconds")]
    double FlushIntervalSeconds { get; set; }

    [Export("sendFeatureFlagEvent")]
    bool SendFeatureFlagEvent { get; set; }

    [Export("preloadFeatureFlags")]
    bool PreloadFeatureFlags { get; set; }

    [Export("captureApplicationLifecycleEvents")]
    bool CaptureApplicationLifecycleEvents { get; set; }

    [Export("captureScreenViews")]
    bool CaptureScreenViews { get; set; }

    [Export("captureElementInteractions")]
    bool CaptureElementInteractions { get; set; }

    [Export("debug")]
    bool Debug { get; set; }

    [Export("optOut")]
    bool OptOut { get; set; }

    [Export("sessionReplay")]
    bool SessionReplay { get; set; }

    [Export("sessionReplayConfig", ArgumentSemantic.Strong)]
    PostHogSessionReplayConfig SessionReplayConfig { get; }
}

// Swift @objc(PostHogSessionReplayConfig) -> clean ObjC runtime name.
[BaseType(typeof(NSObject), Name = "PostHogSessionReplayConfig")]
interface PostHogSessionReplayConfig
{
    [Export("maskAllTextInputs")]
    bool MaskAllTextInputs { get; set; }

    [Export("maskAllImages")]
    bool MaskAllImages { get; set; }

    [Export("maskAllSandboxedViews")]
    bool MaskAllSandboxedViews { get; set; }

    [Export("captureNetworkTelemetry")]
    bool CaptureNetworkTelemetry { get; set; }

    [Export("screenshotMode")]
    bool ScreenshotMode { get; set; }

    [Export("throttleDelay")]
    double ThrottleDelay { get; set; }

    [Export("captureLogs")]
    bool CaptureLogs { get; set; }

    [NullAllowed, Export("sampleRate", ArgumentSemantic.Strong)]
    NSNumber SampleRate { get; set; }
}

// Swift PostHogSDK keeps its mangled ObjC runtime name.
[BaseType(typeof(NSObject), Name = "_TtC7PostHog10PostHogSDK")]
[DisableDefaultCtor]
interface PostHogSDK
{
    [Static]
    [Export("shared")]
    PostHogSDK Shared { get; }

    [Static]
    [Export("with:")]
    PostHogSDK With(PostHogConfig config);

    [Export("setup:")]
    void Setup(PostHogConfig config);

    [Export("debug:")]
    void Debug(bool enabled);

    [Export("getDistinctId")]
    string GetDistinctId();

    [Export("getAnonymousId")]
    string GetAnonymousId();

    [Export("getDeviceId")]
    string GetDeviceId();

    [Export("getSessionId")]
    [return: NullAllowed]
    string GetSessionId();

    [Export("startSession")]
    void StartSession();

    [Export("endSession")]
    void EndSession();

    [Export("captureDeepLinkWithUrl:")]
    void CaptureDeepLink(NSUrl url);

    [Export("flush")]
    void Flush();

    [Export("reset")]
    void Reset();

    [Export("registerProperties:")]
    void Register(NSDictionary properties);

    [Export("unregisterProperties:")]
    void Unregister(string key);

    [Export("identify:")]
    void Identify(string distinctId);

    [Export("identifyWithDistinctId:userProperties:")]
    void Identify(string distinctId, [NullAllowed] NSDictionary userProperties);

    [Export("identifyWithDistinctId:userProperties:userPropertiesSetOnce:")]
    void Identify(string distinctId, [NullAllowed] NSDictionary userProperties, [NullAllowed] NSDictionary userPropertiesSetOnce);

    [Export("capture:")]
    void Capture(string @event);

    [Export("captureWithEvent:properties:")]
    void Capture(string @event, [NullAllowed] NSDictionary properties);

    [Export("captureWithEvent:properties:userProperties:userPropertiesSetOnce:groups:")]
    void Capture(string @event, [NullAllowed] NSDictionary properties, [NullAllowed] NSDictionary userProperties, [NullAllowed] NSDictionary userPropertiesSetOnce, [NullAllowed] NSDictionary groups);

    [Export("screen:")]
    void Screen(string screenTitle);

    [Export("screenWithTitle:properties:")]
    void Screen(string screenTitle, [NullAllowed] NSDictionary properties);

    [Export("alias:")]
    void Alias(string alias);

    [Export("groupWithType:key:")]
    void Group(string type, string key);

    [Export("groupWithType:key:groupProperties:")]
    void Group(string type, string key, [NullAllowed] NSDictionary groupProperties);

    [Export("reloadFeatureFlags")]
    void ReloadFeatureFlags();

    [Export("reloadFeatureFlagsWithCallback:")]
    void ReloadFeatureFlags(Action callback);

    [Export("getFeatureFlag:")]
    [return: NullAllowed]
    NSObject GetFeatureFlag(string key);

    [Export("isFeatureEnabled:")]
    bool IsFeatureEnabled(string key);

    [Export("getFeatureFlagPayload:")]
    [return: NullAllowed]
    NSObject GetFeatureFlagPayload(string key);

    [Export("optIn")]
    void OptIn();

    [Export("optOut")]
    void OptOut();

    [Export("isOptOut")]
    bool IsOptOut();

    [Export("close")]
    void Close();

    [Export("startSessionRecording")]
    void StartSessionRecording();

    [Export("startSessionRecordingWithResumeCurrent:")]
    void StartSessionRecording(bool resumeCurrent);

    [Export("stopSessionRecording")]
    void StopSessionRecording();

    [Export("isSessionReplayActive")]
    bool IsSessionReplayActive();

    [Export("isAutocaptureActive")]
    bool IsAutocaptureActive();
}
