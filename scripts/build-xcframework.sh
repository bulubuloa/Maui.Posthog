#!/bin/bash
set -euo pipefail
SP="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJ="$SP/posthog-ios/PostHog.xcodeproj"
OUT="$SP/xcf-build"
ARCH="$OUT/archives"
rm -rf "$OUT"; mkdir -p "$ARCH"

archive() {
  local dest="$1" name="$2"
  echo ">>> Archiving PostHog for $name ..."
  xcrun xcodebuild archive \
    SKIP_INSTALL=NO \
    BUILD_LIBRARY_FOR_DISTRIBUTION=YES \
    -project "$PROJ" \
    -scheme PostHog \
    -configuration Release \
    -destination "$dest" \
    -archivePath "$ARCH/PostHog-$name.xcarchive" \
    -quiet
}

archive "generic/platform=iOS"           "iOS"
archive "generic/platform=iOS Simulator" "iOS_Simulator"

echo ">>> Framework locations:"
find "$ARCH" -name "PostHog.framework" -maxdepth 5

echo ">>> Creating XCFramework ..."
DEV="$ARCH/PostHog-iOS.xcarchive/Products/Library/Frameworks/PostHog.framework"
SIM="$ARCH/PostHog-iOS_Simulator.xcarchive/Products/Library/Frameworks/PostHog.framework"
rm -rf "$OUT/PostHog.xcframework"
xcrun xcodebuild -create-xcframework \
  -framework "$DEV" \
  -framework "$SIM" \
  -output "$OUT/PostHog.xcframework"

echo ">>> DONE: $OUT/PostHog.xcframework"
ls -la "$OUT/PostHog.xcframework"
