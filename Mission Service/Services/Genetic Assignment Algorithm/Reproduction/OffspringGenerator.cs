using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Crossover;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Mutation;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Selection;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Reproduction;

public class OffspringGenerator : IOffspringGenerator
{
    private readonly ISelectionStrategy _parentSelectionStrategy;
    private readonly ICrossoverStrategy _crossoverStrategy;
    private readonly IMutationStrategy _mutationStrategy;
    private readonly AssignmentAlgorithmConfiguration _algorithmConfig;

    public OffspringGenerator(
        ISelectionStrategy parentSelectionStrategy,
        ICrossoverStrategy crossoverStrategy,
        IMutationStrategy mutationStrategy,
        IOptions<AssignmentAlgorithmConfiguration> algorithmConfig
    )
    {
        _parentSelectionStrategy = parentSelectionStrategy;
        _crossoverStrategy = crossoverStrategy;
        _mutationStrategy = mutationStrategy;
        _algorithmConfig = algorithmConfig.Value;
    }

    public List<AssignmentChromosome> CreateOffspring(
        List<AssignmentChromosome> parentPopulation,
        int numberOfOffspringToGenerate,
        List<UAV> availableUAVs
    )
    {
        int numberOfParentPairs = CalculateNumberOfParentPairs(numberOfOffspringToGenerate);
        AssignmentChromosome[] generatedOffspring = new AssignmentChromosome[numberOfOffspringToGenerate];

        Parallel.For(0, numberOfParentPairs, currentPairIndex =>
        {
            AssignmentChromosome firstSelectedParent = SelectParentFromPopulation(parentPopulation);
            AssignmentChromosome secondSelectedParent = SelectParentFromPopulation(parentPopulation);

            CrossoverResult crossoverResult = PerformCrossoverOnParents(firstSelectedParent, secondSelectedParent);

            ApplyMutationToChromosome(crossoverResult.FirstChromosome, availableUAVs);
            ApplyMutationToChromosome(crossoverResult.SecondChromosome, availableUAVs);

            int firstOffspringPosition = CalculateOffspringArrayPosition(currentPairIndex, isFirstOffspring: true);
            int secondOffspringPosition = CalculateOffspringArrayPosition(currentPairIndex, isFirstOffspring: false);

            generatedOffspring[firstOffspringPosition] = crossoverResult.FirstChromosome;

            if (secondOffspringPosition < numberOfOffspringToGenerate)
            {
                generatedOffspring[secondOffspringPosition] = crossoverResult.SecondChromosome;
            }
        });

        return FilterOutNullChromosomes(generatedOffspring);
    }

    private int CalculateNumberOfParentPairs(int numberOfOffspring)
    {
        return (numberOfOffspring + 1) / MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE;
    }

    private AssignmentChromosome SelectParentFromPopulation(List<AssignmentChromosome> population)
    {
        return _parentSelectionStrategy.SelectParentChromosome(population);
    }

    private CrossoverResult PerformCrossoverOnParents(
        AssignmentChromosome firstParent,
        AssignmentChromosome secondParent
    )
    {
        bool shouldPerformCrossover = Random.Shared.NextDouble() < _algorithmConfig.CrossoverProbability;

        if (shouldPerformCrossover)
        {
            return _crossoverStrategy.CrossoverChromosomes(firstParent, secondParent);
        }

        return new CrossoverResult { FirstChromosome = firstParent, SecondChromosome = secondParent };
    }

    private void ApplyMutationToChromosome(AssignmentChromosome chromosome, List<UAV> availableUAVs)
    {
        bool shouldMutate = Random.Shared.NextDouble() < _algorithmConfig.MutationProbability;

        if (shouldMutate)
        {
            _mutationStrategy.MutateChromosome(chromosome, availableUAVs);
        }
    }

    private int CalculateOffspringArrayPosition(int pairIndex, bool isFirstOffspring)
    {
        int basePosition = pairIndex * MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE;
        int offsetIndex = isFirstOffspring
            ? MissionServiceConstants.MainAlgorithm.FIRST_OFFSPRING_INDEX
            : MissionServiceConstants.MainAlgorithm.SECOND_OFFSPRING_INDEX;

        return basePosition + offsetIndex;
    }

    private List<AssignmentChromosome> FilterOutNullChromosomes(AssignmentChromosome[] chromosomeArray)
    {
        return chromosomeArray.Where(chromosome => chromosome != null).ToList();
    }
}
