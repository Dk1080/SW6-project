using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.AI;

namespace FitnessApi.Endpoints.Tools;

public class PriceTools
{
    
    [Description("Calculates the price of MIKKEL protein bars, returning a value in danish kroners.")]
    public int CalculatePrice([Description("The number of MIKKEL protein bars to calculate a price for"), Required()]int count)
    {
        return count * 10;
    }

    public void Toast()
    {
        Console.WriteLine("!!!!Toast!!!!");
        return;
    }
}