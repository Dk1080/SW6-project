using FitnessApp.Models.System_DTOs;
using FitnessApp.Services;


namespace FitnessApp.PlatformsImplementations;

internal class HealthService : IHealthService
{
    public Task DebugInsertSteps(int Steps)
    {
        throw new NotImplementedException();
    }

    public Task<List<HealthHourInfo>> GetSteps()
    {
        throw new NotImplementedException();
    }

    public Task<PermissionStatus> RequestPermission()
    {
        throw new NotImplementedException();
    }

}

