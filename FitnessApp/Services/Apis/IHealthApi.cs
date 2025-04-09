using FitnessApp.Models.System_DTOs;
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
        [Post("/chat")]
        Task<HttpResponseMessage> sendHealthData(HealthInfoDTO healthInfo);
    }
}
