using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;

namespace Mission_Service.Extensions
{
    public static class EntitiesExtentions
    {
        public static Assignment ToEntity(this ApplyAssignmentDto dto)
        {
            return new Assignment
            {
                SuggestedAssignments = dto.SuggestedAssignments.ToList(),
                ActualAssignments = dto.ActualAssignments.ToList(),
                CreatedAt = DateTime.UtcNow,
            };
        }

        public static AssignmentRo ToRo(this Assignment assignment)
        {
            return new AssignmentRo
            {
                SuggestedAssignments = assignment.SuggestedAssignments,
                ActualAssignments = assignment.ActualAssignments,
            };
        }
    }
}
