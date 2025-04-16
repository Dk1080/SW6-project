using System.Text.Json;
using FitnessApi.Models;
using FitnessApi.Services;

namespace FitnessApi.Endpoints.Tools;

public class DatabaseTools
{

    public string GetFitnessData(HealthInfo healthInfo)
    {
        string output = string.Join(", ", healthInfo.HourInfos);
        return output;
    }

    public async Task<string> SetPreferencesAndGoals(string username, string chartPreference, string goalType, string value, IUserPreferencesService userPreferencesService)
    {
        Console.WriteLine($"Preference and goals inputs: {username}, {chartPreference}, {goalType}, {value}");
        UserPreferences preferences = new UserPreferences();
        preferences.User = username;
        preferences.ChartPreference = chartPreference;

        List<Goal> goals = new List<Goal>();
        goals.Add(new Goal() { GoalType = goalType, Value = int.Parse(value) });
        preferences.Goals = goals;

        try
        {
            var updatedPreferences = await userPreferencesService.UpdateUserPreferencesAsync(username, preferences);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "Failure to update preferences";
        }
        return "Success, the users preferences and goals have been updated";
    }
}