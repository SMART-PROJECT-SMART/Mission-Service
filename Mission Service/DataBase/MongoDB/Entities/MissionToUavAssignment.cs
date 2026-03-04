using Core.Common.Enums;
using Mission_Service.Models;
using MongoDB.Bson.Serialization.Attributes;

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

        public Dictionary<TelemetryFields, double> UavTelemetrySnapshot { get; set; } = new();
    }
}
