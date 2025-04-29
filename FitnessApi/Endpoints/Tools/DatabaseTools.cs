using System.ComponentModel;
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

    [Description("chartPreference may only have either the value Halfcircle or the value Column, interval must be weekly or monthly, endDate must be in yyyy-MM-dd format (e.g., 2025-04-24).")]
    public async Task<string> SetPreferencesAndGoals(IUserPreferencesService userPreferencesService, string username, string chartPreference="none", string goalType="none", string value="none", string interval="none", string endDate="none")
    {
        Console.WriteLine($"Preference and goals inputs: {username}, {chartPreference}, {goalType}, {value}, {interval}, {endDate}");
        UserPreferences preferences = await userPreferencesService.GetUserPreferencesAsync(username);
        if (preferences == null)
        {
            preferences = new UserPreferences();
        }

        if (chartPreference != "none" && chartPreference != goalType)
        {
            if (chartPreference == "Halfcircle" || chartPreference == "Column")
            {
                preferences.ChartPreference = chartPreference;    
            }
            else
            {
                return "Chart preference was not one of the following allowed values: Halfcircle, Column";
            }
        }

        if (goalType != "none" && value != "none")
        {
            List<Goal> goals = new List<Goal>();
            DateTime goalStartDate = DateTime.UtcNow.Date;   
            goals.Add(new Goal() { GoalType = goalType, Value = int.Parse(value), Interval = interval, StartDate = goalStartDate, EndDate = DateTime.Parse(endDate) });
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