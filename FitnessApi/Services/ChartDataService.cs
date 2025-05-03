using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApi.Services
{
    public class ChartDataService : IChartDataService
    {
        private readonly DatabaseContext _dbContext;

        public ChartDataService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ChartData>> GetChartDataAsync(string username)
        {
            Console.WriteLine("Inside GetChartDataAsync");
            try
            {
                var userPref = _dbContext.UserPreferences.FirstOrDefault(p => p.User == username);

                var goalInfo = userPref.Goals.FirstOrDefault();
                
                Console.WriteLine($"Retrieved GoalInfo: {goalInfo?.GoalType}");

                if (goalInfo == null)
                {
                    Console.WriteLine("No goal info found.");
                    return new List<ChartData>();
                }
                
                //Get HealthHourInfo with correct datatype - TODO 
                var matchingHourInfos = _dbContext.HealthInfo
                    .AsQueryable()
                    .Where(h => h.Username == username)
                    .SelectMany(h => h.HourInfos)
                    .Where(info => info.metricName == goalInfo.GoalType)
                    .GroupBy(h=>h.endTime.Date)
                    .Select(g=>g.ToList())
                    .ToList();
                
                Console.WriteLine($"Total matching HealthHourInfo records: {matchingHourInfos.Count}");

                
                //FOR CORRECT DATA, CREATE ChartData objects per day
                List<ChartData> chartData = new List<ChartData>();
                foreach (var day in matchingHourInfos)
                {
                    ChartData chartDay = new ChartData();
                    chartDay.Date = day.First().endTime.Date;
                    chartDay.Value = 0;
                    foreach (var healthhourinfo in day)
                    {
                        chartDay.Value += healthhourinfo.dataCount;
                    }

                    chartData.Add(chartDay);
                }
                
                Console.WriteLine($"chartData: {chartData}");

                return chartData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}