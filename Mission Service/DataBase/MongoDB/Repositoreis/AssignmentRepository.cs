using Mission_Service.Common.Constants;
using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.Extensions;
using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;
using Mission_Service.DataBase.MongoDB.Repositoreis.Interfaces;
using MongoDB.Driver;

namespace Mission_Service.DataBase.MongoDB.Repositoreis
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IMongoCollection<Assignment> _assignmentCollection;

        public AssignmentRepository(IMongoDatabase mongoDatabase)
        {
            _assignmentCollection = mongoDatabase.GetCollection<Assignment>(
                MissionServiceConstants.MongoDB.ASSIGNMENTS_COLLECTION
            );
        }

        public async Task<bool> SaveAssignmentAsync(ApplyAssignmentDto applyAssignmentDto)
        {
            Assignment assignmentEntity = applyAssignmentDto.ToEntity();
            await _assignmentCollection.InsertOneAsync(assignmentEntity);

            return !string.IsNullOrEmpty(assignmentEntity.Id);
        }

        public async Task<AssignmentRo?> GetAssignmentByIdAsync(string assignmentId)
        {
            FilterDefinition<Assignment> idFilter = Builders<Assignment>.Filter.Eq(assignment => assignment.Id, assignmentId);

            Assignment? assignmentEntity = await _assignmentCollection
                .Find(idFilter)
                .FirstOrDefaultAsync();

            return assignmentEntity?.ToRo();
        }

        public async Task<IEnumerable<AssignmentRo>> GetAllAssignmentsAsync(int skipCount = 0, int limitCount = 100)
        {
            List<Assignment> assignmentEntities = await _assignmentCollection
                .Find(FilterDefinition<Assignment>.Empty)
                .SortByDescending(assignment => assignment.CreatedAt)
                .Skip(skipCount)
                .Limit(limitCount)
                .ToListAsync();

            return assignmentEntities.Select(assignmentEntity => assignmentEntity.ToRo());
        }

        public async Task<bool> DeleteAssignmentAsync(string assignmentId)
        {
            FilterDefinition<Assignment> idFilter = Builders<Assignment>.Filter.Eq(assignment => assignment.Id, assignmentId);

            DeleteResult deleteResult = await _assignmentCollection.DeleteOneAsync(idFilter);

            return deleteResult.DeletedCount > 0;
        }
    }
}
