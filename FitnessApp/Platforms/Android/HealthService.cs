using Android.Graphics;
using Android.Health.Connect.DataTypes;
using AndroidX.Health.Connect.Client.Aggregate;
using DTOs;
using FitnessApp.Helpers;
using FitnessApp.Services;
using FitnessApp.ViewModels;
using GoogleGson;
using Java.Lang;
using Java.Time;
using Kotlin.Coroutines;
using Microsoft.Maui.Controls.PlatformConfiguration;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Double = System.Double;
using HealthConnect = NewBindingAndroid.DotnetNewBinding;
using Object = Java.Lang.Object;


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

    public async Task<List<HealthHourInfo>> GetSteps()
    {
        var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
        CancellationTokenSource cts = new CancellationTokenSource();


        IContinuation continuation = new Continuation(taskCompletionSource, default);


        //Set the current date to java format
        DateTime currentDate = DateTime.Now;
        string currentFormattedDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss");


        //Date to start reading from TODO set custom date based on already avalible data.
        DateTime startDate = DateTime.Now.AddDays(-30);
        string startFormattedDate = startDate.ToString("yyyy-MM-dd HH:mm:ss");

        //Format the time
        var formatter = Java.Time.Format.DateTimeFormatter.OfPattern("yyyy-MM-dd HH:mm:ss");
        var currentDataJava = LocalDateTime.Parse(currentFormattedDate, formatter);
        var startDateJava = LocalDateTime.Parse(startFormattedDate, formatter);



        //Ask for the data from the phone
        healthConnect.GetData(startDateJava, currentDataJava, "StepRecord", continuation);


        //Wait for the task to complete
        var result = await taskCompletionSource.Task;


        Console.WriteLine(result.GetType().FullName);

        //Variable to hold return values.
        List<HourStepInfo> hourStepInfos = new();


        if (result is Android.Runtime.JavaList javaList) { 

        
            foreach (HealthConnect.DotnetStepDTO item in javaList)
            {
                //Create a new converter object to make java data usable in dotnet.
                HourStepInfo tmpObj = new(item.StartTime,item.EndTime,item.DataCount);
                hourStepInfos.Add(tmpObj);
            }

            //Convert to list of HealthHourInfo and return TODO change location of this when there is more data.
            var returnList = new List<HealthHourInfo>();
            foreach (var item in hourStepInfos)
            {
                returnList.Add(new HealthHourInfo(item.startTime, item.endTime, item.dataCount));
            }

            return returnList;
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

class HourStepInfo{

    public DateTime startTime {  get; set; }
    public DateTime endTime { get; set; }
    public Double dataCount { get; set; }
    

    public HourStepInfo(Instant startTime, Instant endTime, Double dataCount)
    {
       
        //Convert the java time to C# time.
        this.startTime = DateTimeOffset.FromUnixTimeMilliseconds(startTime.ToEpochMilli()).DateTime;
        this.endTime = DateTimeOffset.FromUnixTimeMilliseconds(endTime.ToEpochMilli()).DateTime;
        this.dataCount = dataCount;
    }

}