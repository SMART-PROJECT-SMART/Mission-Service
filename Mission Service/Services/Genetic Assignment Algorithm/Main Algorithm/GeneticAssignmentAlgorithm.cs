using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Extensions;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Execution;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness_Calculator;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Population.Population_Initilizer;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Repair.Pipeline;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Reproduction;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Selection.Elite;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Main_Algorithm
{
    public class GeneticAssignmentAlgorithm : IAssignmentAlgorithm
    {
        private readonly IFitnessCalculator _fitnessCalculator;
        private readonly IPopulationInitializer _populationInitializer;
        private readonly IEliteSelector _eliteSelector;
        private readonly IOffspringGenerator _offspringGenerator;
        private readonly IParallelExecutor _parallelExecutor;
        private readonly IRepairPipeline _repairPipeline;
        private readonly AssignmentAlgorithmConfiguration _algorithmConfig;

        public GeneticAssignmentAlgorithm(
            IFitnessCalculator fitnessCalculator,
            IPopulationInitializer populationInitializer,
            IEliteSelector eliteSelector,
            IOffspringGenerator offspringGenerator,
            IParallelExecutor parallelExecutor,
            IRepairPipeline repairPipeline,
            IOptions<AssignmentAlgorithmConfiguration> algorithmConfig
        )
        {
            _fitnessCalculator = fitnessCalculator;
            _populationInitializer = populationInitializer;
            _eliteSelector = eliteSelector;
            _offspringGenerator = offspringGenerator;
            _parallelExecutor = parallelExecutor;
            _repairPipeline = repairPipeline;
            _algorithmConfig = algorithmConfig.Value;
        }

        public AssignmentResult PreformAssignmentAlgorithm(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            List<Mission> allMissions = missions.ToList();
            List<UAV> allAvailableUAVs = uavs.ToList();

            List<Mission> missionsWithCompatibleUAVs = FilterMissionsWithCompatibleUAVs(
                allMissions,
                allAvailableUAVs
            );

            List<AssignmentChromosome> currentPopulation = _populationInitializer
                .CreateInitialPopulation(missionsWithCompatibleUAVs, allAvailableUAVs)
                .ToList();

            _parallelExecutor.EvaluatePopulationFitnessInParallel(
                currentPopulation,
                chromosome => _fitnessCalculator.CalculateFitness(chromosome)
            );

            AssignmentChromosome bestChromosomeFound =
                currentPopulation.FindChromosomeWithHighestFitness();
            int generationsWithoutImprovement = 0;

            for (
                int currentGeneration = 0;
                ShouldContinueEvolution(generationsWithoutImprovement, currentGeneration);
                currentGeneration++
            )
            {
                currentPopulation = CreateNextGenerationPopulation(
                    currentPopulation,
                    missionsWithCompatibleUAVs,
                    allAvailableUAVs
                );

                _parallelExecutor.EvaluatePopulationFitnessInParallel(
                    currentPopulation,
                    chromosome => _fitnessCalculator.CalculateFitness(chromosome)
                );

                AssignmentChromosome bestChromosomeInGeneration =
                    currentPopulation.FindChromosomeWithHighestFitness();

                if (
                    IsNewChromosomeBetterThanCurrent(
                        bestChromosomeInGeneration,
                        bestChromosomeFound
                    )
                )
                {
                    bestChromosomeFound = bestChromosomeInGeneration;
                    generationsWithoutImprovement = 0;
                }
                else
                {
                    generationsWithoutImprovement++;
                }
            }

            return new AssignmentResult(new[] { bestChromosomeFound });
        }

        private List<AssignmentChromosome> CreateNextGenerationPopulation(
            List<AssignmentChromosome> currentPopulation,
            List<Mission> missions,
            List<UAV> availableUAVs
        )
        {
            List<AssignmentChromosome> eliteChromosomes = _eliteSelector.SelectElite(
                currentPopulation,
                _algorithmConfig.ElitePrecentage,
                _algorithmConfig.PopulationSize
            );

            int numberOfOffspringNeeded = _algorithmConfig.PopulationSize - eliteChromosomes.Count;
            List<AssignmentChromosome> generatedOffspring = _offspringGenerator.CreateOffspring(
                currentPopulation,
                numberOfOffspringNeeded,
                availableUAVs
            );

            _parallelExecutor.RepairPopulationInParallel(
                generatedOffspring,
                chromosome =>
                    _repairPipeline.RepairChromosomeViolaitions(chromosome, missions, availableUAVs)
            );

            List<AssignmentChromosome> validOffspringChromosomes =
                generatedOffspring.FilterValidChromosomes();
            List<AssignmentChromosome> invalidOffspringChromosomesSortedByQuality =
                generatedOffspring.FilterAndOrderInvalidChromosomesByQuality();

            List<AssignmentChromosome> combinedOffspring = new List<AssignmentChromosome>();
            combinedOffspring.AddRange(validOffspringChromosomes);

            int remainingPopulationSlots =
                numberOfOffspringNeeded - validOffspringChromosomes.Count;
            if (remainingPopulationSlots > 0 && invalidOffspringChromosomesSortedByQuality.Any())
            {
                combinedOffspring.AddRange(
                    invalidOffspringChromosomesSortedByQuality.Take(remainingPopulationSlots)
                );
            }

            List<AssignmentChromosome> nextGenerationPopulation = new List<AssignmentChromosome>(
                _algorithmConfig.PopulationSize
            );
            nextGenerationPopulation.AddRange(eliteChromosomes);
            nextGenerationPopulation.AddRange(combinedOffspring);

            return nextGenerationPopulation;
        }

        private static List<Mission> FilterMissionsWithCompatibleUAVs(
            List<Mission> allMissions,
            List<UAV> availableUAVs
        )
        {
            HashSet<UAVType> availableUAVTypes = availableUAVs
                .Select(uav => uav.UavType)
                .ToHashSet();

            return allMissions
                .Where(mission => availableUAVTypes.Contains(mission.RequiredUAVType))
                .ToList();
        }

        private bool IsNewChromosomeBetterThanCurrent(
            AssignmentChromosome newChromosome,
            AssignmentChromosome currentBestChromosome
        )
        {
            double fitnessImprovement =
                newChromosome.FitnessScore - currentBestChromosome.FitnessScore;
            return fitnessImprovement
                > MissionServiceConstants.MainAlgorithm.NO_IMPROVEMENT_THRESHOLD;
        }

        private bool ShouldContinueEvolution(
            int generationsWithoutImprovement,
            int currentGenerationIndex
        )
        {
            bool hasNotStagnated = generationsWithoutImprovement < _algorithmConfig.StagnationLimit;
            bool hasNotReachedMaxGenerations =
                currentGenerationIndex < _algorithmConfig.MaxGenerations;

            return hasNotStagnated && hasNotReachedMaxGenerations;
        }
    }
}
