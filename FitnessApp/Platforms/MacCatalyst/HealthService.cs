using FitnessApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

