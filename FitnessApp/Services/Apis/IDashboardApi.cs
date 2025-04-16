using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApi.Models.Api_DTOs;

namespace FitnessApp.Services.Apis
{
    public interface IDashboardApi
    {
        [Get("/getChartData")]  
        Task<List<ChartDataDTO>> GetChartData();

        [Get("/getUserPreferences")]
        Task<UserPreferencesDTO> GetUserPreferences();
    }
}
    