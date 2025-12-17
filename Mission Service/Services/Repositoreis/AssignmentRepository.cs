using Mission_Service.Common.Constants;
using Mission_Service.Extensions;
using Mission_Service.Models.Dto;
using Mission_Service.Models.Entities;
using Mission_Service.Models.Ro;
using Mission_Service.Services.Repositoreis.Interfaces;
using MongoDB.Driver;

namespace Mission_Service.Services.Repositoreis
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IMongoCollection<Assignment> _assignmentCollection;

        public AssignmentRepository(IMongoDatabase mongoDatabase)
        {
            _assignmentCollection = mongoDatabase.GetCollection<Assignment>(MissionServiceConstants.MongoDB.ASSIGNMENTS_COLLECTION);
        }

        public async Task SaveAssignmentAsync(ApplyAssignmentDto applyAssignmentDto)
        {
            Assignment assignmentToSave = applyAssignmentDto.ToEntity();
            await _assignmentCollection.InsertOneAsync(assignmentToSave);
        }

        public async Task<AssignmentRo?> GetAssignmentByIdAsync(string assignmentId)
        {
            FilterDefinition<Assignment> idFilter = Builders<Assignment>.Filter.Eq(a => a.Id, assignmentId);
            Assignment? assignment = await _assignmentCollection
                .Find(idFilter)
                .FirstOrDefaultAsync();

            return assignment?.ToRo();
        }

        public async Task<IEnumerable<AssignmentRo>> GetAllAssignmentsAsync(int skip = 0, int limit = 100)
        {
            List<Assignment> assignments = await _assignmentCollection
                .Find(FilterDefinition<Assignment>.Empty)
                .SortByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();

            return assignments.Select(a => a.ToRo());
        }

        public async Task<bool> DeleteAssignmentAsync(string assignmentId)
        {
            FilterDefinition<Assignment> idFilter = Builders<Assignment>.Filter.Eq(a => a.Id, assignmentId);
            DeleteResult deleteResult = await _assignmentCollection.DeleteOneAsync(idFilter);

            return deleteResult.DeletedCount > 0;
        }
    }
}
