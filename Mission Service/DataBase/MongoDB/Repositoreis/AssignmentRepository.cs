using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.DataBase.MongoDB.Entities;
using Mission_Service.DataBase.MongoDB.Repositoreis.Interfaces;
using Mission_Service.Extensions;
using Mission_Service.Models.Dto;
using Mission_Service.Models.Ro;
using MongoDB.Driver;

namespace Mission_Service.DataBase.MongoDB.Repositoreis
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IMongoCollection<Assignment> _assignmentCollection;

        public AssignmentRepository(
            IMongoClient mongoClient,
            IOptions<MongoDBConfiguration> mongoDbConfig
        )
        {
            IMongoDatabase database = mongoClient.GetDatabase(mongoDbConfig.Value.DatabaseName);
            _assignmentCollection = database.GetCollection<Assignment>(
                MissionServiceConstants.MongoDB.ASSIGNMENTS_COLLECTION
            );
        }

        public async Task<bool> SaveAssignmentAsync(
            ApplyAssignmentDto applyAssignmentDto,
            CancellationToken cancellationToken = default
        )
        {
            Assignment assignmentEntity = applyAssignmentDto.ToEntity();
            await _assignmentCollection.InsertOneAsync(assignmentEntity, null, cancellationToken);

            return !string.IsNullOrEmpty(assignmentEntity.Id);
        }
    }
}
