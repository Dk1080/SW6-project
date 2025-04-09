using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Collections.Generic;

namespace FitnessApi.Services
{
    public class HealthDataService : IHealthDataService
    {

        private readonly DatabaseContext _databaseContext;

        public HealthDataService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        public ObjectId AddHealthInfo(HealthInfo healthInfo)
        {
            _databaseContext.HealthInfo.Add(healthInfo);
            _databaseContext.SaveChanges();
            return healthInfo.Id;
        }

        public void DeleteHealthInfo(HealthInfo healthInfo)
        {
            throw new NotImplementedException();
        }

        public HealthInfo? GetHealthInfoByID(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public HealthInfo GetHealthInfoForUser(string UserName)
        {
            return _databaseContext.HealthInfo.FirstOrDefault(healthInfo => healthInfo.Username == UserName);
        }


        public void UpdateHealthInfo(HealthInfo healthInfo)
        {
            var existingHealthInfo = _databaseContext.HealthInfo
                .Include(hi => hi.HourInfos)
                .FirstOrDefault(hi => hi.Id == healthInfo.Id);

            if (existingHealthInfo != null)
            {
                // Ensure the incoming HourInfos are distinct
                var distinctNewHourInfos = healthInfo.HourInfos
                    .GroupBy(hi => new { hi.startTime, hi.endTime })
                    .Select(g => g.First())
                    .ToList();

                var newHealthData = distinctNewHourInfos
                    .Where(newData => !existingHealthInfo.HourInfos.Any(existingInfo =>
                        existingInfo.endTime.ToUniversalTime().Ticks == newData.endTime.ToUniversalTime().Ticks &&
                        existingInfo.startTime.ToUniversalTime().Ticks == newData.startTime.ToUniversalTime().Ticks))
                    .ToList();

                //Add the new data to the database
                existingHealthInfo.HourInfos.AddRange(newHealthData);

                _databaseContext.Entry(existingHealthInfo).State = EntityState.Modified;
                _databaseContext.SaveChanges();
            }
        }

    }
}
