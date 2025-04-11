using FitnessApi.Models;
using DTOs;
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
                HttpContext context) =>
            {
                // få brugeren fra current session :3
                string? username = context.Session.GetString("Username");

                try
                {
                    var chartData = await ChartDataService.GetChartDataAsync(username);

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
