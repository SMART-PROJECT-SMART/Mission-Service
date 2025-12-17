using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mission_Service.Models.Entities
{
    public class Assignment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRequired]
        public List<MissionToUavAssignment> SuggestedAssignments { get; set; } = new();

        [BsonRequired]
        public List<MissionToUavAssignment> ActualAssignments { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
