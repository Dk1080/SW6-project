using System.Collections.Generic;

namespace FitnessApi.Models.Api_DTOs
{
    public class UserPreferencesDTO
    {
        public string ChartPreference { get; set; }
        public List<GoalDTO> Goals { get; set; } = new List<GoalDTO>();
    }

    public class GoalDTO
    {
        public string GoalType { get; set; }
        public int Value { get; set; }
    }
}