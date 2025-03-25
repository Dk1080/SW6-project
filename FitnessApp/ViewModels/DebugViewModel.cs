using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnesApp.Helpers;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration;



#if ANDROID
using Java.Lang;
using Java.Util.Concurrent;
using Java.Interop;
using Kotlin.Jvm.Functions;
using Kotlin.Coroutines;
using Android.Health.Connect;
using Android.Health.Connect.DataTypes;
using Android.Runtime;
using Android.App;
using Testing = NewBindingAndroid.DotnetNewBinding;
#endif

namespace FitnesApp.ViewModels
{
    public partial class DebugViewModel : ObservableObject
    {

#if ANDROID
        [ObservableProperty]
        string labelText = "Hello, " + Testing.GetString("hello");

        Testing test = new Testing(Platform.AppContext);


#endif


         [ObservableProperty]
        int x = 0;


        [RelayCommand]
        public void Increment()
        {
            X++;
        }

        [RelayCommand]
        public async Task GetSteps()
        {
            //Call Binding function


#if ANDROID

            var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();


            IContinuation continuation = new Continuation(taskCompletionSource, default);

            // Call the InsertSteps method with the continuation
            test.InsertSteps(continuation);
#endif
        }

        [RelayCommand]
        async Task RequestPermissions()
        {
            await CheckAndRequestLocationPermission();
        }


        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<MyHealthPermission>();
            if (status == PermissionStatus.Granted)
                return status;
            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }
            if (Permissions.ShouldShowRationale<MyHealthPermission>())
            {
                // Prompt the user with additional information as to why the permission is needed
                await Shell.Current.DisplayAlert("Needs permissions", "BECAUSE!!!", "OK");
            }
            status = await Permissions.RequestAsync<MyHealthPermission>();
            return status;
        }



    }


#if ANDROID
    //Source https://github.com/Kebechet/Maui.Health/blob/74f862d786bfb983e5e683e4a719e556f99edc84/src/Maui.Health/Platforms/Android/Callbacks/KotlinResolver.cs
    public class Continuation : Java.Lang.Object, IContinuation
    {
        public ICoroutineContext Context => EmptyCoroutineContext.Instance;

        private readonly TaskCompletionSource<Java.Lang.Object> _taskCompletionSource;

        public Continuation(TaskCompletionSource<Java.Lang.Object> taskCompletionSource, CancellationToken cancellationToken)
        {
            _taskCompletionSource = taskCompletionSource;
            cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
        }

        public void ResumeWith(Java.Lang.Object result)
        {
            //Check if there are any exception. We don't have access to the class Kotlin.Result.Failure. But we can extraxt the exception (Throwable) from the field in the class.
            var exceptionField = result.Class.GetDeclaredFields().FirstOrDefault(x => x.Name.Contains("exception", StringComparison.OrdinalIgnoreCase));
            if (exceptionField != null)
            {
                var exception = Java.Interop.JavaObjectExtensions.JavaCast<Throwable>(exceptionField.Get(result));
                _taskCompletionSource.TrySetException(new System.Exception(exception.Message));
                return;
            }
            else
            {
                _taskCompletionSource.TrySetResult(result);
            }
        }
    }
#endif




}
