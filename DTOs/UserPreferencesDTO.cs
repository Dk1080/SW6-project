using System.Collections.Generic;

namespace FitnessApi.Models.Api_DTOs
{
    public class UserPreferencesDTO
    {
        public string ChartPreference { get; set; } = "Column"; // Default
        public List<string> Goals { get; set; } = new List<string>();
    }
}