using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Extensions;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Evolution.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Reproduction.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Evolution
{
    public class StandardEvolutionStrategy : IEvolutionStrategy
    {
        private readonly IEliteSelector _eliteSelector;
        private readonly IOffspringGenerator _offspringGenerator;
        private readonly IParallelExecutor _parallelExecutor;
        private readonly IRepairPipeline _repairPipeline;
        private readonly AssignmentAlgorithmConfiguration _algorithmConfig;

        public StandardEvolutionStrategy(
            IEliteSelector eliteSelector,
            IOffspringGenerator offspringGenerator,
            IParallelExecutor parallelExecutor,
            IRepairPipeline repairPipeline,
            IOptions<AssignmentAlgorithmConfiguration> algorithmConfig
        )
        {
            _eliteSelector = eliteSelector;
            _offspringGenerator = offspringGenerator;
            _parallelExecutor = parallelExecutor;
            _repairPipeline = repairPipeline;
            _algorithmConfig = algorithmConfig.Value;
        }

        public IReadOnlyList<AssignmentChromosome> CreateNextGeneration(
            IReadOnlyList<AssignmentChromosome> currentPopulation,
            IEnumerable<Mission> missions,
            IEnumerable<UAV> availableUAVs
        )
        {
            IEnumerable<AssignmentChromosome> eliteChromosomes = SelectEliteChromosomes(
                currentPopulation
            );

            int numberOfOffspringNeeded = CalculateOffspringNeeded(eliteChromosomes);

            IEnumerable<AssignmentChromosome> generatedOffspring = GenerateAndRepairOffspring(
                currentPopulation,
                numberOfOffspringNeeded,
                missions,
                availableUAVs
            );

            IEnumerable<AssignmentChromosome> filteredOffspring = FilterAndCombineOffspring(
                generatedOffspring,
                numberOfOffspringNeeded
            );

            return AssembleNextGeneration(eliteChromosomes, filteredOffspring);
        }

        private IEnumerable<AssignmentChromosome> SelectEliteChromosomes(
            IReadOnlyList<AssignmentChromosome> currentPopulation
        )
        {
            return _eliteSelector.SelectElite(
                currentPopulation,
                _algorithmConfig.ElitePrecentage,
                _algorithmConfig.PopulationSize
            );
        }

        private int CalculateOffspringNeeded(IEnumerable<AssignmentChromosome> eliteChromosomes)
        {
            return _algorithmConfig.PopulationSize - eliteChromosomes.Count();
        }

        private IEnumerable<AssignmentChromosome> GenerateAndRepairOffspring(
            IReadOnlyList<AssignmentChromosome> currentPopulation,
            int numberOfOffspringNeeded,
            IEnumerable<Mission> missions,
            IEnumerable<UAV> availableUAVs
        )
        {
            IEnumerable<AssignmentChromosome> generatedOffspring =
                _offspringGenerator.CreateOffspring(
                    currentPopulation,
                    numberOfOffspringNeeded,
                    availableUAVs as IReadOnlyList<UAV> ?? availableUAVs.ToList()
                );

            _parallelExecutor.RepairPopulationInParallel(
                generatedOffspring,
                chromosome =>
                    _repairPipeline.RepairChromosomeViolaitions(chromosome, missions, availableUAVs)
            );

            return generatedOffspring;
        }

        private IEnumerable<AssignmentChromosome> FilterAndCombineOffspring(
            IEnumerable<AssignmentChromosome> generatedOffspring,
            int numberOfOffspringNeeded
        )
        {
            IEnumerable<AssignmentChromosome> validOffspring =
                generatedOffspring.FilterValidChromosomes();

            IEnumerable<AssignmentChromosome> invalidOffspringSortedByQuality =
                generatedOffspring.FilterAndOrderInvalidChromosomesByQuality();

            List<AssignmentChromosome> combinedOffspring = new List<AssignmentChromosome>();
            combinedOffspring.AddRange(validOffspring);

            int remainingSlots = numberOfOffspringNeeded - combinedOffspring.Count;

            if (remainingSlots > 0 && invalidOffspringSortedByQuality.Any())
            {
                combinedOffspring.AddRange(invalidOffspringSortedByQuality.Take(remainingSlots));
            }

            return combinedOffspring;
        }

        private IReadOnlyList<AssignmentChromosome> AssembleNextGeneration(
            IEnumerable<AssignmentChromosome> eliteChromosomes,
            IEnumerable<AssignmentChromosome> offspring
        )
        {
            List<AssignmentChromosome> nextGeneration = new List<AssignmentChromosome>(
                _algorithmConfig.PopulationSize
            );

            nextGeneration.AddRange(eliteChromosomes);
            nextGeneration.AddRange(offspring);

            return nextGeneration;
        }
    }
}
