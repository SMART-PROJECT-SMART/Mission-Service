using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Execution;

public class ParallelExecutor : IParallelExecutor
{
    public void EvaluatePopulationFitnessInParallel(
        List<AssignmentChromosome> chromosomePopulationToEvaluate,
        Action<AssignmentChromosome> evaluateSingleChromosomeFitness
    )
    {
        ParallelOptions parallelExecutionOptions = CreateParallelExecutionOptions();

        Parallel.ForEach(
            chromosomePopulationToEvaluate,
            parallelExecutionOptions,
            evaluateSingleChromosomeFitness
        );
    }

    public void RepairPopulationInParallel(
        List<AssignmentChromosome> chromosomePopulationToRepair,
        Action<AssignmentChromosome> repairSingleChromosome
    )
    {
        ParallelOptions parallelExecutionOptions = CreateParallelExecutionOptions();

        Parallel.ForEach(
            chromosomePopulationToRepair,
            parallelExecutionOptions,
            repairSingleChromosome
        );
    }

    private ParallelOptions CreateParallelExecutionOptions()
    {
        return new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
    }
}
