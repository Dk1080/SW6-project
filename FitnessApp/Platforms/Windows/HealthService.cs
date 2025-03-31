using FitnessApp.Services;


namespace FitnessApp.PlatformsImplementations;

internal class HealthService : IHealthService
{
    public Task DebugInsertSteps(int Steps)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetSteps()
    {
        throw new NotImplementedException();
    }

    public Task<PermissionStatus> RequestPermission()
    {
        throw new NotImplementedException();
    }
}

