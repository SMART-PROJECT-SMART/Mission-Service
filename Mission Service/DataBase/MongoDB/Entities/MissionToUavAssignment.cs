using Core.Common.Enums;
using Mission_Service.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Mission_Service.DataBase.MongoDB.Entities
{
    public class MissionToUavAssignment
    {
        [BsonRequired]
        public Mission Mission { get; set; } = new();

        [BsonRequired]
        public int UavTailId { get; set; }

        [BsonRequired]
        public DateTime StartTime { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<TelemetryFields, double> UavTelemetrySnapshot { get; set; } = new();
    }
}
