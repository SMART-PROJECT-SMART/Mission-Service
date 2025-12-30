using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Extensions;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Evolution.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm
{
    public class GeneticAssignmentAlgorithm : IAssignmentAlgorithm
    {
        private readonly IFitnessCalculator _fitnessCalculator;
        private readonly IPopulationInitializer _populationInitializer;
        private readonly IEvolutionStrategy _evolutionStrategy;
        private readonly IParallelExecutor _parallelExecutor;
        private readonly IRepairPipeline _repairPipeline;
        private readonly AssignmentAlgorithmConfiguration _algorithmConfig;

        public GeneticAssignmentAlgorithm(
            IFitnessCalculator fitnessCalculator,
            IPopulationInitializer populationInitializer,
            IEvolutionStrategy evolutionStrategy,
            IParallelExecutor parallelExecutor,
            IRepairPipeline repairPipeline,
            IOptions<AssignmentAlgorithmConfiguration> algorithmConfig
        )
        {
            _fitnessCalculator = fitnessCalculator;
            _populationInitializer = populationInitializer;
            _evolutionStrategy = evolutionStrategy;
            _parallelExecutor = parallelExecutor;
            _repairPipeline = repairPipeline;
            _algorithmConfig = algorithmConfig.Value;
        }

        public AssignmentResult PreformAssignmentAlgorithm(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            IEnumerable<Mission> missionsWithCompatibleUAVs = FilterMissionsWithCompatibleUAVs(
                missions,
                uavs
            );

            IReadOnlyList<AssignmentChromosome> currentPopulation = InitializePopulation(
                missionsWithCompatibleUAVs,
                uavs
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
                currentPopulation = _evolutionStrategy.CreateNextGeneration(
                    currentPopulation,
                    missionsWithCompatibleUAVs,
                    uavs
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

            return new AssignmentResult(bestChromosomeFound);
        }

        private IReadOnlyList<AssignmentChromosome> InitializePopulation(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            IReadOnlyList<AssignmentChromosome> initialPopulation = _populationInitializer
                .CreateInitialPopulation(missions, uavs)
                .ToArray();

            _parallelExecutor.RepairPopulationInParallel(
                initialPopulation,
                chromosome =>
                    _repairPipeline.RepairChromosomeViolaitions(chromosome, missions, uavs)
            );

            _parallelExecutor.EvaluatePopulationFitnessInParallel(
                initialPopulation,
                chromosome => _fitnessCalculator.CalculateFitness(chromosome)
            );

            return initialPopulation;
        }

        private static IEnumerable<Mission> FilterMissionsWithCompatibleUAVs(
            IEnumerable<Mission> allMissions,
            IEnumerable<UAV> availableUAVs
        )
        {
            HashSet<UAVType> availableUAVTypes = availableUAVs
                .Select(uav => uav.UavType)
                .ToHashSet();

            return allMissions.Where(mission =>
                availableUAVTypes.Contains(mission.RequiredUAVType)
            );
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
