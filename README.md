# Maui.Posthog

[![NuGet](https://img.shields.io/nuget/v/Omnicasa.Maui.PostHog.svg)](https://www.nuget.org/packages/Omnicasa.Maui.PostHog)
[![Downloads](https://img.shields.io/nuget/dt/Omnicasa.Maui.PostHog.svg)](https://www.nuget.org/packages/Omnicasa.Maui.PostHog)
[![Publish NuGet](https://github.com/bulubuloa/Maui.Posthog/actions/workflows/publish.yml/badge.svg)](https://github.com/bulubuloa/Maui.Posthog/actions/workflows/publish.yml)

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

## Overriding behavior

`PostHogService` is `public` on each platform and every member is `virtual`. Subclass it
(under `Platforms/iOS` or `Platforms/Android`), override what you need, call `base` to keep the
native behavior, and register your type:

```csharp
public class MyPostHog : PostHogService
{
    public MyPostHog(PostHogOptions options) : base(options) { }

    public override void Capture(string eventName, IDictionary<string, object>? properties = null)
    {
        properties ??= new Dictionary<string, object>();
        properties["app_build"] = AppInfo.BuildString;
        base.Capture(eventName, properties);   // Client is available via the protected property
    }
}

// register the subclass:
builder.AddPostHog<MyPostHog>(o => o.ApiKey = "phc_your_project_key");
```

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
