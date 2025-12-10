using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Execution;

public interface IParallelExecutor
{
    void EvaluatePopulationFitnessInParallel(
        List<AssignmentChromosome> populationToEvaluate,
        Action<AssignmentChromosome> fitnessEvaluationAction
    );

    List<AssignmentChromosome> GenerateOffspringInParallel(
        List<AssignmentChromosome> parentPopulation,
        int numberOfOffspringToGenerate,
        Func<List<AssignmentChromosome>, (AssignmentChromosome parent1, AssignmentChromosome parent2)> selectParentPair,
        Func<AssignmentChromosome, AssignmentChromosome, (AssignmentChromosome offspring1, AssignmentChromosome offspring2)> createOffspringFromParents
    );

    void RepairPopulationInParallel(
        List<AssignmentChromosome> populationToRepair,
        Action<AssignmentChromosome> chromosomeRepairAction
    );
}
