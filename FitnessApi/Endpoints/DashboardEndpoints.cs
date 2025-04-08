using FitnessApi.Models;
using FitnessApi.Models.Api_DTOs;
using FitnessApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApi.Endpoints
{
    public static class DashboardEndpoints
    {
        public static void MapDashboardEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/getChartData", async (
                [FromServices] IChartDataService ChartDataService,
                [FromServices] IUserService userService,// så man få den currently logged in user >_<
                HttpContext context) =>
            {
                // få brugeren fra current session :3
                string? username = context.Session.GetString("Username");


                // Få user objektet for at få id'et som så så senere searcher databasen efter for at finde graf data UwU
                var user = userService.GetUserByName(username);
                if (user == null)
                {
                    return Results.Unauthorized(); // hvis user not found OwO
                }

                string userId = user.Id.ToString();

                try
                {
                    var chartData = await ChartDataService.GetChartDataAsync(userId);

                    if (chartData == null || !chartData.Any())
                    {
                        return Results.Ok(new List<ChartDataDTO>());
                    }

                    var chartDataDtos = chartData.Select(w => new ChartDataDTO
                    {
                        Date = w.Date.ToString("yyyy-MM-dd"),
                        Value = w.Value
                    }).ToList();

                    return Results.Ok(chartDataDtos);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error fetching chart data: {ex.Message}");
                }
            })
            .WithName("GetChartData")
            .Produces<List<ChartDataDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
