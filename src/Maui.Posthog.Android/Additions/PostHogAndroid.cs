using Android.Runtime;

namespace Com.Posthog.Android;

// The Kotlin `companion object` (setup/with) isn't surfaced by the binding generator
// because its methods are generic. Expose the companion instance via its static JNI field.
partial class PostHogAndroid
{
    private static Companion? _shared;

    public static Companion Shared => _shared ??= ResolveCompanion();

    private static Companion ResolveCompanion()
    {
        var cls = JNIEnv.FindClass("com/posthog/android/PostHogAndroid");
        try
        {
            var fieldId = JNIEnv.GetStaticFieldID(cls, "Companion", "Lcom/posthog/android/PostHogAndroid$Companion;");
            var peer = JNIEnv.GetStaticObjectField(cls, fieldId);
            return Java.Lang.Object.GetObject<Companion>(peer, JniHandleOwnership.TransferLocalRef)!;
        }
        finally
        {
            JNIEnv.DeleteLocalRef(cls);
        }
    }
}
