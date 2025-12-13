using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Execution;

public interface IParallelExecutor
{
    void EvaluatePopulationFitnessInParallel(
        IReadOnlyList<AssignmentChromosome> chromosomePopulationToEvaluate,
        Action<AssignmentChromosome> evaluateSingleChromosomeFitness
    );

    void RepairPopulationInParallel(
        IEnumerable<AssignmentChromosome> chromosomePopulationToRepair,
        Action<AssignmentChromosome> repairSingleChromosome
    );
}
