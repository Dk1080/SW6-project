using System.ComponentModel;
using System.Text.Json;
using FitnessApi.Models;
using FitnessApi.Services;
using Google.Protobuf.WellKnownTypes;

namespace FitnessApi.Endpoints.Tools;

public class DatabaseTools
{

    public string GetFitnessData(HealthInfo healthInfo)
    {
        string output = string.Join(", ", healthInfo.HourInfos);
        return output;
    }


    [Description("Sets or updates user preferences and goals. You can provide chartPreference alone to update the chart preference, or provide goalType, value, interval, and endDate to update goals. Do NOT ask for userPreferencesService or username. " +
                 "chartPreference may only be 'Circle' or 'Column'. If updating chartPreference, only provide this parameter. " +
                 "To update goals, goalType must be one of: ActiveCaloriesBurnedRecord, TotalCaloriesBurnedRecord, DistanceRecord, ElevationGainedRecord, FloorsClimbedRecord, HeartRateRecord, HeightRecord, RestingHeartRateRecord, StepsRecord, WeightRecord, WheelchairPushesRecord. " +
                 "interval must be 'weekly', 'biweekly', or 'monthly'. endDate must be in yyyy-MM-dd format (e.g., 2025-04-24).")]
    public async Task<string> SetPreferencesAndGoals(IUserPreferencesService userPreferencesService, string username, string chartPreference = "none", string goalType = "none", string value = "none", string interval = "none", string endDate = "none")
    {
        Console.WriteLine($"Preference and goals inputs: {username}, {chartPreference}, {goalType}, {value}, {interval}, {endDate}");

        // Fetch or initialize preferences
        UserPreferences preferences = await userPreferencesService.GetUserPreferencesAsync(username);
        if (preferences == null)
        {
            preferences = new UserPreferences
            {
                User = username,
                Goals = new List<Goal>()
            };
        }

        bool preferencesUpdated = false;
        List<string> updateResults = new List<string>();

        
        // Update ChartPreference if provided
        // ChartPreference kan opdateres alene
        if (chartPreference != "none")
        {
            if (chartPreference != "Circle" && chartPreference != "Column")
            {
                return "Chart preference must be 'Circle' or 'Column'.";
            }
            preferences.ChartPreference = chartPreference;
            preferencesUpdated = true;
            updateResults.Add("Chart preference updated");
        }

        // Check goal parameters 
        bool isGoalUpdateRequested = goalType != "none" || value != "none" || interval != "none" || endDate != "none";

        if (isGoalUpdateRequested)
        {
            // HVIS MAN VIL OPDATERE EN GOAL SKAL MAN BRUGE ALT INFO
            if (goalType == "none" || value == "none" || interval == "none" || endDate == "none")
            {
                return "To update or add a goal, provide all goal-related parameters: goalType, value, interval, and endDate.";
            }

            // Validate 
            var validGoalTypes = new[] { "ActiveCaloriesBurnedRecord", "TotalCaloriesBurnedRecord", "DistanceRecord", "ElevationGainedRecord", "FloorsClimbedRecord", "HeartRateRecord", "HeightRecord", "RestingHeartRateRecord", "StepsRecord", "WeightRecord", "WheelchairPushesRecord" };
            if (!validGoalTypes.Contains(goalType))
            {
                return $"Goal type must be one of: {string.Join(", ", validGoalTypes)}.";
            }

            // Validate 
            if (!int.TryParse(value, out int goalValue) || goalValue <= 0)
            {
                return "Value must be a positive integer.";
            }

            // Validate 
            if (interval != "weekly" && interval != "biweekly" && interval != "monthly")
            {
                return "Interval must be 'weekly', 'biweekly', or 'monthly'.";
            }

            // Validate 
            if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime goalEndDate))
            {
                return "End date must be in yyyy-MM-dd format.";
            }
            if (goalEndDate.Date < DateTime.UtcNow.Date)
            {
                return "End date must be in the future.";
            }

            // Update or add goal
            var existingGoal = preferences.Goals.FirstOrDefault(g => g.GoalType == goalType);
            if (existingGoal != null)
            {
                // Update existing goal
                existingGoal.Value = goalValue;
                existingGoal.Interval = interval;
                existingGoal.StartDate = DateTime.UtcNow.Date;
                existingGoal.EndDate = goalEndDate;
                updateResults.Add($"Goal for {goalType} updated");
            }
            else
            {
                // Add new goal
                preferences.Goals.Add(new Goal
                {
                    GoalType = goalType,
                    Value = goalValue,
                    Interval = interval,
                    StartDate = DateTime.UtcNow.Date,
                    EndDate = goalEndDate
                });
                updateResults.Add($"Goal for {goalType} added");
            }
            preferencesUpdated = true;
        }

        // Save changes if any updates were made
        if (preferencesUpdated)
        {
            try
            {
                await userPreferencesService.UpdateUserPreferencesAsync(username, preferences);
                return $"Success: {string.Join("; ", updateResults)}.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Failure to update preferences.";
            }
        }

        return "No updates made: Provide valid chartPreference or all goal parameters (goalType, value, interval, endDate).";
    }



    [Description("Updates a user's goal. Must provide goalType, value, interval, and endDate. Do NOT ask for userPreferencesService or username. " +
                 "goalType must be one of: ActiveCaloriesBurnedRecord, TotalCaloriesBurnedRecord, DistanceRecord, ElevationGainedRecord, FloorsClimbedRecord, HeartRateRecord, HeightRecord, RestingHeartRateRecord, StepsRecord, WeightRecord, WheelchairPushesRecord. " +
                 "interval must be 'weekly', 'biweekly', or 'monthly'. endDate must be in yyyy-MM-dd format (e.g., 2025-04-24).")]
    public async Task<string> UpdateGoal(IUserPreferencesService userPreferencesService, string username, string goalType="none", string value="none", string interval="none", string endDate = "none") 
    {
        UserPreferences preferences = await userPreferencesService.GetUserPreferencesAsync(username);

        // Validate value
        if (!int.TryParse(value, out int goalValue) || goalValue <= 0)
        {
            return "Value must be a positive integer.";
        }
        // Validate endDate
        if (!DateTime.TryParseExact(endDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime goalEndDate))
        {
            return "End date must be in yyyy-MM-dd format.";
        }
        if (goalEndDate.Date < DateTime.UtcNow.Date)
        {
            return "End date must be in the future.";
        }

        // Update 
        var existingGoal = preferences.Goals.FirstOrDefault(g => g.GoalType == goalType);
        if (existingGoal != null)
        {
            existingGoal.Value = goalValue;
            existingGoal.Interval = interval;
            existingGoal.StartDate = DateTime.UtcNow.Date;
            existingGoal.EndDate = goalEndDate;
        }
        try
        {
            await userPreferencesService.UpdateUserPreferencesAsync(username, preferences);
            return "Success: Goal updated.";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "Failure to update goal.";
        }

    }
}