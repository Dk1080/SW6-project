using System.Text.Json;
using FitnessApi.Models;

namespace FitnessApi.Endpoints.Tools;

public class DatabaseTools
{
    public string GetFitnessData(HealthInfo healthInfo)
    {
        string output = string.Join(", ", healthInfo.HourInfos);
        return output;
    }
}