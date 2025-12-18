using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Execution;

public class ParallelExecutor : IParallelExecutor
{
    private readonly ParallelOptions _parallelOptions;

    public ParallelExecutor()
    {
        _parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };
    }

    public void EvaluatePopulationFitnessInParallel(
        IReadOnlyList<AssignmentChromosome> chromosomePopulationToEvaluate,
        Action<AssignmentChromosome> evaluateSingleChromosomeFitness
    )
    {
        Parallel.ForEach(
            chromosomePopulationToEvaluate,
            _parallelOptions,
            evaluateSingleChromosomeFitness
        );
    }

    public void RepairPopulationInParallel(
        IEnumerable<AssignmentChromosome> chromosomePopulationToRepair,
        Action<AssignmentChromosome> repairSingleChromosome
    )
    {
        Parallel.ForEach(chromosomePopulationToRepair, _parallelOptions, repairSingleChromosome);
    }
}
