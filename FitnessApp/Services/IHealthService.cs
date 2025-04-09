using FitnessApp.Models.System_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Services
{
    public interface IHealthService
    {
        Task<PermissionStatus> RequestPermission();

        Task DebugInsertSteps(int Steps);

        Task<List<HealthHourInfo>> GetSteps();
    }
}
