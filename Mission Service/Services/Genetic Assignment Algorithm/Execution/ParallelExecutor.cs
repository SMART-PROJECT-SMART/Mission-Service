using Mission_Service.Common.Constants;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Execution;

public class ParallelExecutor : IParallelExecutor
{
    public void EvaluatePopulationFitnessInParallel(
        List<AssignmentChromosome> populationToEvaluate,
        Action<AssignmentChromosome> fitnessEvaluationAction
    )
    {
        ParallelOptions parallelOptions = CreateParallelOptions();
        Parallel.ForEach(populationToEvaluate, parallelOptions, fitnessEvaluationAction);
    }

    public List<AssignmentChromosome> GenerateOffspringInParallel(
        List<AssignmentChromosome> parentPopulation,
        int numberOfOffspringToGenerate,
        Func<List<AssignmentChromosome>, (AssignmentChromosome parent1, AssignmentChromosome parent2)> selectParentPair,
        Func<AssignmentChromosome, AssignmentChromosome, (AssignmentChromosome offspring1, AssignmentChromosome offspring2)> createOffspringFromParents
    )
    {
        int numberOfParentPairsNeeded = CalculateNumberOfPairsNeeded(numberOfOffspringToGenerate);
        AssignmentChromosome[] offspringArray = new AssignmentChromosome[numberOfOffspringToGenerate];

        Parallel.For(0, numberOfParentPairsNeeded, pairIndex =>
        {
            (AssignmentChromosome firstParent, AssignmentChromosome secondParent) = selectParentPair(parentPopulation);
            (AssignmentChromosome firstOffspring, AssignmentChromosome secondOffspring) = createOffspringFromParents(firstParent, secondParent);

            int firstOffspringIndex = CalculateOffspringIndex(pairIndex, isFirstOffspring: true);
            int secondOffspringIndex = CalculateOffspringIndex(pairIndex, isFirstOffspring: false);

            offspringArray[firstOffspringIndex] = firstOffspring;

            if (secondOffspringIndex < numberOfOffspringToGenerate)
            {
                offspringArray[secondOffspringIndex] = secondOffspring;
            }
        });

        return FilterNullChromosomes(offspringArray);
    }

    public void RepairPopulationInParallel(
        List<AssignmentChromosome> populationToRepair,
        Action<AssignmentChromosome> chromosomeRepairAction
    )
    {
        ParallelOptions parallelOptions = CreateParallelOptions();
        Parallel.ForEach(populationToRepair, parallelOptions, chromosomeRepairAction);
    }

    private ParallelOptions CreateParallelOptions()
    {
        return new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
    }

    private int CalculateNumberOfPairsNeeded(int numberOfOffspring)
    {
        return (numberOfOffspring + 1) / MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE;
    }

    private int CalculateOffspringIndex(int pairIndex, bool isFirstOffspring)
    {
        int baseIndex = pairIndex * MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE;
        int offset = isFirstOffspring
            ? MissionServiceConstants.MainAlgorithm.FIRST_OFFSPRING_INDEX
            : MissionServiceConstants.MainAlgorithm.SECOND_OFFSPRING_INDEX;

        return baseIndex + offset;
    }

    private List<AssignmentChromosome> FilterNullChromosomes(AssignmentChromosome[] chromosomes)
    {
        return chromosomes.Where(chromosome => chromosome != null).ToList();
    }
}
