using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation
{
    public interface IMutationStrategy
    {
        void MutateChromosome(AssignmentChromosome assignmentChromosome, IEnumerable<UAV> uavs);
    }
}
