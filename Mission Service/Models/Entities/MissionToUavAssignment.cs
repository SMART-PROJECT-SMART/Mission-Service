using MongoDB.Bson.Serialization.Attributes;

namespace Mission_Service.Models.Entities
{
    public class MissionToUavAssignment
    {
        [BsonRequired]
        public Mission Mission { get; set; } = new();

        [BsonRequired]
        public int UavTailId { get; set; }
    }
}
