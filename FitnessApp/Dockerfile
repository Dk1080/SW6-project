FROM budtmo/docker-android:latest

USER root

# Copy the APK (relative to SW6)
COPY FitnessApp/FitnessApp.Android/bin/Debug/net8.0-android/com.CompanyName.FitnessApp.apk /app/FitnessApp.Android/FitnessApp.Android.apk

# Verify APK was copied (inside container)
RUN ls -l /app/FitnessApp.Android/FitnessApp.Android.apk

# CMD instruction (only install and list packages)
CMD ["/bin/sh", "-c", "adb wait-for-device && adb install /app/FitnessApp.Android/FitnessApp.Android.apk && adb shell pm list packages | grep com.CompanyName.FitnessApp"]