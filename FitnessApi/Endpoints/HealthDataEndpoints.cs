using DTOs;
using FitnessApi.Models;
using FitnessApi.Services;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace FitnessApi.Endpoints
{
    public static class HealthDataEndpoints
    {
        public static WebApplication MapHealthDataEndpoints(this WebApplication app)
        {

            app.MapPost("/userHealthInfo", (HealthInfoDTO healthInfoDTO, HttpContext httpContext, IHealthDataService healthDataService) =>
            {

                //Check if the user is authorized
                string username = httpContext.Session.GetString("Username");


                if (string.IsNullOrEmpty(username))
                {
                    return Results.Unauthorized();
                }


                try
                {
                    //Store the data in the database

                    var DbHealthInfo = healthDataService.GetHealthInfoForUser(username);
                    //Check if there is already data in the database
                    if (DbHealthInfo != null)
                    {
                        //If so then update the previous data.
                        HealthInfo newHealthInfo = new();
                        newHealthInfo.Id = DbHealthInfo.Id;
                        newHealthInfo.Username = username;
                        newHealthInfo.HourInfos = healthInfoDTO.hourInfos;

                        healthDataService.UpdateHealthInfo(newHealthInfo);
                    }
                    else
                    {
                        //If not then add a new entry
                        HealthInfo newHealthInfo = new();
                        newHealthInfo.Id = ObjectId.Empty;
                        newHealthInfo.Username = username;
                        newHealthInfo.HourInfos = healthInfoDTO.hourInfos;
                        healthDataService.AddHealthInfo(newHealthInfo);
                    }

                    return Results.Ok();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Results.InternalServerError();

                }



            });



            return app;
        }
    }
}
