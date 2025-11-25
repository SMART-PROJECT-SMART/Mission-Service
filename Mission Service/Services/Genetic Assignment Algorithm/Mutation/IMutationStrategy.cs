using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Mutation
{
    public interface IMutationStrategy
    {
        void MutateChromosome(AssignmentChromosome assignmentChromosome, IEnumerable<UAV> uavs);
    }
}
