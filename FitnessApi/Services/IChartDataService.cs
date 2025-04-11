using FitnessApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApi.Services
{
    public interface IChartDataService
    {
        Task<List<ChartData>> GetChartDataAsync(string username);
    }
}