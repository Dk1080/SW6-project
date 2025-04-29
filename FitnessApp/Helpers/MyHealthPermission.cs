#if ANDROID
using Android.Health.Connect;
#endif

using Microsoft.Maui.Controls.PlatformConfiguration;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace FitnessApp.Helpers
{
    class MyHealthPermission : BasePlatformPermission
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new List<(string permission, bool isRuntime)>
            {
                ("android.permission.health.READ_ACTIVE_CALORIES_BURNED", true),
                ("android.permission.health.WRITE_ACTIVE_CALORIES_BURNED", true),
                ("android.permission.health.READ_TOTAL_CALORIES_BURNED", true),
                ("android.permission.health.WRITE_TOTAL_CALORIES_BURNED", true),
                ("android.permission.health.READ_DISTANCE", true),
                ("android.permission.health.WRITE_DISTANCE", true),
                ("android.permission.health.READ_ELEVATION_GAINED", true),
                ("android.permission.health.WRITE_ELEVATION_GAINED", true),
                ("android.permission.health.READ_FLOORS_CLIMBED", true),
                ("android.permission.health.WRITE_FLOORS_CLIMBED", true),
                ("android.permission.health.READ_HEART_RATE", true),
                ("android.permission.health.WRITE_HEART_RATE", true),
                ("android.permission.health.READ_HEIGHT", true),
                ("android.permission.health.WRITE_HEIGHT", true),
                ("android.permission.health.READ_RESTING_HEART_RATE", true),
                ("android.permission.health.WRITE_RESTING_HEART_RATE", true),
                ("android.permission.health.READ_STEPS", true),
                ("android.permission.health.WRITE_STEPS", true),
                ("android.permission.health.READ_WEIGHT", true),
                ("android.permission.health.WRITE_WEIGHT", true),
                ("android.permission.health.READ_WHEELCHAIR_PUSHES", true),
                ("android.permission.health.WRITE_WHEELCHAIR_PUSHES", true),
            }.ToArray();
#endif
    }
}
