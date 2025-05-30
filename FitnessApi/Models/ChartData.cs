﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessApi.Models
{
    public class ChartData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("value")]
        public Double Value { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }
        
    }
}