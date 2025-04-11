using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApi.Services
{
    public class ChartDataService : IChartDataService
    {
        private readonly DatabaseContext _dbContext;

        public ChartDataService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ChartData>> GetChartDataAsync(string username)
        {
            return await _dbContext.ChartData
                .Where(w => w.UserId == username)
                .OrderBy(w => w.Date)
                .ToListAsync();
        }
    }
}