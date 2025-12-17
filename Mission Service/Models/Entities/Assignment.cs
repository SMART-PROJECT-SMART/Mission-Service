using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mission_Service.Models.Entities
{
    public class Assignment
    {
        [BsonId]
        [BsonRequired]
        public string Id { get; set; } = string.Empty;

        [BsonRequired]
        public List<MissionToUavAssignment> SuggestedAssignments { get; set; } = new();

        [BsonRequired]
        public List<MissionToUavAssignment> ActualAssignments { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
