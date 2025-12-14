using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm
{
    public interface IAssignmentAlgorithm
    {
        public AssignmentResult PreformAssignmentAlgorithm(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        );
    }
}
