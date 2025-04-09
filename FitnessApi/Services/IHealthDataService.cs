using FitnessApi.Models;
using MongoDB.Bson;

namespace FitnessApi.Services
{
    public interface IHealthDataService
    {


        HealthInfo? GetHealthInfoByID(ObjectId id);


        public HealthInfo GetHealthInfoForUser(string UserName);

        ObjectId AddHealthInfo(HealthInfo healthInfo);

        void UpdateHealthInfo(HealthInfo healthInfo);

        void DeleteHealthInfo(HealthInfo healthInfo);



    }
}
