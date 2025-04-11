using DTOs;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Models
{
    public class HealthInfo
    {

        [BsonId]
        public ObjectId Id { get; set; }


        [Required(ErrorMessage = "User has to have a name!")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Has to have a HealthHistory!")]
        [Display(Name = "HealthHistory")]
        public List<HealthHourInfo> HourInfos { get; set; }




    }
}
