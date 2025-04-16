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

    public async Task<string> SetPreferencesAndGoals(IUserPreferencesService userPreferencesService, string username, string chartPreference="none", string goalType="none", string value="none")
    {
        Console.WriteLine($"Preference and goals inputs: {username}, {chartPreference}, {goalType}, {value}");
        UserPreferences preferences = await userPreferencesService.GetUserPreferencesAsync(username);

        if (chartPreference != "none" && chartPreference != goalType)
        {
            preferences.ChartPreference = chartPreference;   
        }

        if (goalType != "none" && value != "none")
        {
            List<Goal> goals = new List<Goal>();
            goals.Add(new Goal() { GoalType = goalType, Value = int.Parse(value) });
            preferences.Goals = goals;   
        }

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