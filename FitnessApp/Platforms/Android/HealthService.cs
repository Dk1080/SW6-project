using FitnessApp.Helpers;
using FitnessApp.Services;
using FitnessApp.ViewModels;
using Java.Lang;
using Kotlin.Coroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using HealthConnect = NewBindingAndroid.DotnetNewBinding;


namespace FitnessApp.PlatformsImplementations;

internal class HealthService : IHealthService
{
    private HealthConnect healthConnect;

    public HealthService() {
        //Create an object to interact with the healthconect library.
        healthConnect = new HealthConnect(Platform.AppContext);
    }


    public async Task DebugInsertSteps(int Steps)
    {
        var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();


        IContinuation continuation = new Continuation(taskCompletionSource, default);

        // Call the InsertSteps method with the continuation
        healthConnect.InsertSteps(Steps,continuation);

        
    }

    public async Task<int> GetSteps()
    {
        var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
        CancellationTokenSource cts = new CancellationTokenSource();


        IContinuation continuation = new Continuation(taskCompletionSource, default);

        //Start the task to get the step data from the last five days.
        healthConnect.AggregateSteps(
            GetDateTimeMilliseconds(DateTime.Now.AddDays(-5)),
            GetDateTimeMilliseconds(DateTime.Now),
            continuation);


        //Wait for the task to complete
        var result = await taskCompletionSource.Task;

        //Convert from java object to long and then int.
        
        if(result is Java.Lang.Long longResult)
        {
            //Return the step data
            return (int)longResult.LongValue();
        }
        else
        {
            throw new System.Exception("Converion or data retival problem");
        }


    }

    public async Task<PermissionStatus> RequestPermission()
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




    //For java time converation
    public static long GetDateTimeMilliseconds(DateTime dateTime)
    {
   
        DateTime Utc = dateTime;
        // Calculate milliseconds since the Unix epoch
        long millisecondsSinceEpoch = (long)Utc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        return millisecondsSinceEpoch;
    }



}




//Source https://github.com/Kebechet/Maui.Health/blob/74f862d786bfb983e5e683e4a719e556f99edc84/src/Maui.Health/Platforms/Android/Callbacks/Continuation.cs
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
