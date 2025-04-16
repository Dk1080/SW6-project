// FitnessApi/Models/UserPreferences.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace FitnessApi.Models
{
    public class UserPreferences
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user")]
        public string User { get; set; } 

        [BsonElement("chartPreference")]
        public string ChartPreference { get; set; } 

        [BsonElement("Goals")]
        public List<Goal> Goals { get; set; } = new List<Goal>();
    }

    public class Goal
    {
        [BsonElement("goalType")]
        public string GoalType { get; set; }

        [BsonElement("value")]
        public int Value { get; set; }
    }
}