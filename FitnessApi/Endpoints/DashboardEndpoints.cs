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
                [FromQuery] string userId) =>
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.BadRequest("UserId is required.");
                }

                try
                {
                    var workoutData = await ChartDataService.GetChartDataAsync(userId);

                    if (workoutData == null || !workoutData.Any())
                    {
                        return Results.Ok(new List<ChartDataDTO>());
                    }

                    var workoutDataDtos = workoutData.Select(w => new ChartDataDTO
                    {
                        Date = w.Date.ToString("yyyy-MM-dd"),
                        Value = w.Value
                    }).ToList();

                    return Results.Ok(workoutDataDtos);
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
