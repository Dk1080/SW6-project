using DTOs;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Services.Apis
{
    public interface IHealthApi
    {
        [Post("/userHealthInfo")]
        Task<HttpResponseMessage> sendHealthData(HealthInfoDTO healthInfo);
    }
}
