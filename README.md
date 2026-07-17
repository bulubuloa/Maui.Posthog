# Maui.Posthog

.NET MAUI bindings for the native PostHog SDKs, with a single cross-platform façade.

| Platform | Native SDK | Version |
| --- | --- | --- |
| Android (`net10.0-android`) | [`com.posthog:posthog-android`](https://github.com/PostHog/posthog-android) | 3.19.0 |
| iOS (`net10.0-ios`) | [`posthog-ios`](https://github.com/PostHog/posthog-ios) (`PostHog.xcframework`) | 3.64.6 |

Features: **event capture, feature flags, session replay, autocapture / screen views.**

## Projects

```
src/
  Maui.Posthog.Android/   Java binding of posthog-android (+ curtains, posthog core)
  Maui.Posthog.iOS/       ObjC binding of PostHog.xcframework (built from source)
  Maui.Posthog/           Shared IPostHog façade + DI (net10.0-ios;net10.0-android)
samples/
  Maui.Posthog.Sample/    Minimal MAUI app wiring AddPostHog
```

## Usage

Register in `MauiProgram`:

```csharp
builder.AddPostHog(options =>
{
    options.ApiKey = "phc_your_project_key";
    options.Host = "https://eu.i.posthog.com";   // or https://us.i.posthog.com
    options.CaptureScreenViews = true;
    options.Autocapture = true;                   // iOS UI element interactions
    options.SessionReplay = true;
    options.Replay.MaskAllTextInputs = true;
});
```

Then inject `IPostHog` anywhere:

```csharp
public class HomeViewModel(IPostHog posthog)
{
    public void OnPurchase() =>
        posthog.Capture("purchase", new Dictionary<string, object>
        {
            ["plan"] = "pro",
            ["amount"] = 49.0,
        });
}
```

`IPostHog` surface: `Capture`, `Identify`, `Screen`, `Alias`, `Group`, `IsFeatureEnabled`,
`GetFeatureFlag`, `GetFeatureFlagPayload`, `ReloadFeatureFlags`, `Register`/`Unregister`,
`DistinctId`, `Reset`, `Flush`, `OptIn`/`OptOut`, `StartSessionReplay`/`StopSessionReplay`.

## Rebuilding the iOS xcframework

`PostHog.xcframework` under `src/Maui.Posthog.iOS/` is prebuilt from `posthog-ios` 3.64.6.
To regenerate against a newer tag, archive the `PostHog` scheme for device + simulator with
`BUILD_LIBRARY_FOR_DISTRIBUTION=YES`, then `xcodebuild -create-xcframework`
(see `scripts/build-xcframework.sh`). The binding surface lives in
`src/Maui.Posthog.iOS/ApiDefinition.cs`.

## Notes

- iOS binds the `@objc` surface of the Swift SDK. `PostHogSDK` keeps its mangled Swift runtime
  name (`_TtC7PostHog10PostHogSDK`); `PostHogConfig` / `PostHogSessionReplayConfig` use clean names.
- Android reaches the Kotlin `PostHogAndroid` companion via a small JNI shim
  (`Additions/PostHogAndroid.cs`) because the generator does not surface generic companion methods.
