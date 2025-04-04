using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Models.Api_DTOs;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApp.Services.Apis
{
    public interface IDashboardApi
    {
        [Get("/getChartData")]  
        Task<List<ChartDataDTO>> GetChartData([Query] string userId);
    }
}
