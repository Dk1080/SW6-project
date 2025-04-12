using System.Text.Json;
using FitnessApi.Models;

namespace FitnessApi.Endpoints.Tools;

public class DatabaseTools
{
    public string GetFitnessData(User user)
    {
        //user.FitnessData assumed to be List<Dictionary<string,string>>
        string output = JsonSerializer.Serialize(user.FitnessData);
        return output;
        //return "User has ran 10 kilometers every single day for the last week!";
    }
}