using FitnessApi.Models;
using DTOs;
using FitnessApi.Services;
using Microsoft.AspNetCore.Mvc;
using FitnessApi.Models.Api_DTOs;

namespace FitnessApi.Endpoints
{
    public static class DashboardEndpoints
    {
        public static void MapDashboardEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/getChartData", async (
                [FromServices] IChartDataService ChartDataService,
                HttpContext context) =>
            {
                // få brugeren fra current session :3
                string? username = context.Session.GetString("Username");
                Console.WriteLine($"[UserPreferences] in /getChartData Session Username: {username}");

                try
                {
                    var chartData = await ChartDataService.GetChartDataAsync(username);
                    Console.WriteLine($"[chartData] Fetched {chartData?.Count ?? 0} items for {username}");


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


            endpoints.MapGet("/getUserPreferences", async (
                [FromServices] IUserPreferencesService userPreferencesService,
                HttpContext context) =>
            {
                string? username = context.Session.GetString("Username");
                Console.WriteLine($"[UserPreferences] in /getUserPreferences Session Username: {username}");
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("[UserPreferences] No session, returning 401");
                    return Results.Unauthorized();
                }

                try
                {
                    var preferences = await userPreferencesService.GetUserPreferencesAsync(username);
                    Console.WriteLine($"Preferences for {username}: ChartPreference={preferences?.ChartPreference}, Goals={preferences?.Goals?.Count ?? 0}");
                    var preferencesDto = new UserPreferencesDTO
                    {
                        ChartPreference = preferences?.ChartPreference ?? "",
                        Goals = preferences?.Goals.Select(g => new GoalDTO
                        {
                            GoalType = g.GoalType,
                            Value = g.Value,
                            Interval = g.Interval,
                            StartDate = g.StartDate,
                            EndDate = g.EndDate
                        }).ToList() ?? new List<GoalDTO>()
                    };

                    return Results.Ok(preferencesDto);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Error fetching user preferences: {ex.Message}");
                }
            })
            .WithName("GetUserPreferences")
            .Produces<UserPreferencesDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        }



    }
}
