using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Execution;

public interface IParallelExecutor
{
    void EvaluatePopulationFitnessInParallel(
        List<AssignmentChromosome> chromosomePopulationToEvaluate,
        Action<AssignmentChromosome> evaluateSingleChromosomeFitness
    );

    void RepairPopulationInParallel(
        List<AssignmentChromosome> chromosomePopulationToRepair,
        Action<AssignmentChromosome> repairSingleChromosome
    );
}
