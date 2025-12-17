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
            _assignmentCollection = mongoDatabase.GetCollection<Assignment>(
                MissionServiceConstants.MongoDB.ASSIGNMENTS_COLLECTION
            );
        }

        public async Task SaveAssignmentAsync(ApplyAssignmentDto applyAssignmentDto)
        {
            Assignment assignmentToSave = applyAssignmentDto.ToEntity();
            await _assignmentCollection.InsertOneAsync(assignmentToSave);
        }

        public Task<AssignmentRo?> GetAssignmentByIdAsync(string assignmentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AssignmentRo>> GetAllAssignmentsAsync(int skip = 0, int limit = 100)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAssignmentAsync(string assignmentId)
        {
            throw new NotImplementedException();
        }
    }
}
