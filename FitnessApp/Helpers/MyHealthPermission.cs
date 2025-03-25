#if ANDROID
using Android.Health.Connect;
#endif

using Microsoft.Maui.Controls.PlatformConfiguration;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace FitnesApp.Helpers
{
    class MyHealthPermission : BasePlatformPermission
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new List<(string permission, bool isRuntime)>
            {
            ("android.permission.health.READ_STEPS", true),
            ("android.permission.health.WRITE_STEPS", true)
            }.ToArray();
#endif

    }
}
