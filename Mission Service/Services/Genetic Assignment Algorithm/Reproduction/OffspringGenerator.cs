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

    public IEnumerable<AssignmentChromosome> CreateOffspring(
        IReadOnlyList<AssignmentChromosome> parentChromosomePopulation,
        int numberOfOffspringChromosomesToGenerate,
        IReadOnlyList<UAV> availableUAVsForAssignment
    )
    {
        int numberOfParentPairsToCreate = CalculateNumberOfParentPairsNeeded(
            numberOfOffspringChromosomesToGenerate
        );
        AssignmentChromosome[] generatedOffspringArray = new AssignmentChromosome[
            numberOfOffspringChromosomesToGenerate
        ];

        Parallel.For(
            0,
            numberOfParentPairsToCreate,
            currentPairIndexBeingProcessed =>
            {
                AssignmentChromosome firstSelectedParent = SelectParentChromosomeFromPopulation(
                    parentChromosomePopulation
                );
                AssignmentChromosome secondSelectedParent = SelectParentChromosomeFromPopulation(
                    parentChromosomePopulation
                );

                CrossoverResult offspringFromCrossover = PerformCrossoverOnParentPair(
                    firstSelectedParent,
                    secondSelectedParent
                );

                ApplyMutationToChromosome(
                    offspringFromCrossover.FirstChromosome,
                    availableUAVsForAssignment
                );
                ApplyMutationToChromosome(
                    offspringFromCrossover.SecondChromosome,
                    availableUAVsForAssignment
                );

                int firstOffspringPositionInArray = CalculateOffspringArrayPosition(
                    currentPairIndexBeingProcessed,
                    isFirstOffspring: true
                );
                int secondOffspringPositionInArray = CalculateOffspringArrayPosition(
                    currentPairIndexBeingProcessed,
                    isFirstOffspring: false
                );

                generatedOffspringArray[firstOffspringPositionInArray] =
                    offspringFromCrossover.FirstChromosome;

                if (secondOffspringPositionInArray < numberOfOffspringChromosomesToGenerate)
                {
                    generatedOffspringArray[secondOffspringPositionInArray] =
                        offspringFromCrossover.SecondChromosome;
                }
            }
        );

        return FilterOutNullChromosomesFromArray(generatedOffspringArray);
    }

    private int CalculateNumberOfParentPairsNeeded(int numberOfOffspringNeeded)
    {
        return (numberOfOffspringNeeded + 1)
            / MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE;
    }

    private AssignmentChromosome SelectParentChromosomeFromPopulation(
        IReadOnlyList<AssignmentChromosome> parentPopulation
    )
    {
        return _parentSelectionStrategy.SelectParentChromosome(parentPopulation);
    }

    private CrossoverResult PerformCrossoverOnParentPair(
        AssignmentChromosome firstParentChromosome,
        AssignmentChromosome secondParentChromosome
    )
    {
        bool shouldPerformCrossover =
            Random.Shared.NextDouble() < _algorithmConfig.CrossoverProbability;

        if (shouldPerformCrossover)
        {
            return _crossoverStrategy.CrossoverChromosomes(
                firstParentChromosome,
                secondParentChromosome
            );
        }

        return new CrossoverResult
        {
            FirstChromosome = firstParentChromosome,
            SecondChromosome = secondParentChromosome,
        };
    }

    private void ApplyMutationToChromosome(
        AssignmentChromosome chromosomeToMutate,
        IReadOnlyList<UAV> availableUAVs
    )
    {
        bool shouldApplyMutation =
            Random.Shared.NextDouble() < _algorithmConfig.MutationProbability;

        if (shouldApplyMutation)
        {
            _mutationStrategy.MutateChromosome(chromosomeToMutate, availableUAVs);
        }
    }

    private int CalculateOffspringArrayPosition(int parentPairIndex, bool isFirstOffspring)
    {
        int basePositionInArray =
            parentPairIndex * MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE;
        int offsetFromBasePosition = isFirstOffspring
            ? MissionServiceConstants.MainAlgorithm.FIRST_OFFSPRING_INDEX
            : MissionServiceConstants.MainAlgorithm.SECOND_OFFSPRING_INDEX;

        return basePositionInArray + offsetFromBasePosition;
    }

    private IEnumerable<AssignmentChromosome> FilterOutNullChromosomesFromArray(
        AssignmentChromosome[] chromosomeArray
    )
    {
        return chromosomeArray.Where(chromosome => chromosome != null);
    }
}
