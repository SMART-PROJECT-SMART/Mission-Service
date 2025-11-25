using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Crossover;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness_Calculator;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Mutation;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Population.Population_Initilizer;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Repair.Pipeline;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Selection;
using Microsoft.Extensions.Logging;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Main_Algorithm
{
    public class GeneticAssignmentAlgorithm : IAssignmentAlgorithm
    {
        private readonly IFitnessCalculator _fitnessCalculator;
        private readonly IPopulationInitializer _populationInitializer;
        private readonly ISelectionStrategy _selectionStrategy;
        private readonly ICrossoverStrategy _crossoverStrategy;
        private readonly IMutationStrategy _mutationStrategy;
        private readonly IRepairPipeline _repairPipeline;
        private readonly AssignmentAlgorithmConfiguration _algorithmConfiguration;
        private readonly ILogger<GeneticAssignmentAlgorithm> _logger;

        public GeneticAssignmentAlgorithm(
            IFitnessCalculator fitnessCalculator,
            IPopulationInitializer populationInitializer,
            ISelectionStrategy selectionStrategy,
            ICrossoverStrategy crossoverStrategy,
            IMutationStrategy mutationStrategy,
            IRepairPipeline repairPipeline,
            IOptions<AssignmentAlgorithmConfiguration> algorithmConfiguration,
            ILogger<GeneticAssignmentAlgorithm> logger
        )
        {
            _fitnessCalculator = fitnessCalculator;
            _populationInitializer = populationInitializer;
            _selectionStrategy = selectionStrategy;
            _crossoverStrategy = crossoverStrategy;
            _mutationStrategy = mutationStrategy;
            _repairPipeline = repairPipeline;
            _algorithmConfiguration = algorithmConfiguration.Value;
            _logger = logger;
        }

        public AssignmentResult PreformAssignmentAlgorithm(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            List<Mission> missionList = missions.ToList();
            List<UAV> uavList = uavs.ToList();

            _logger.LogInformation("========================================");
            _logger.LogInformation("Starting Genetic Assignment Algorithm");
            _logger.LogInformation($"Missions: {missionList.Count}, UAVs: {uavList.Count}");
            _logger.LogInformation($"Population Size: {_algorithmConfiguration.PopulationSize}");
            _logger.LogInformation($"Max Generations: {_algorithmConfiguration.MaxGenerations}");
            _logger.LogInformation("========================================");

            List<AssignmentChromosome> population = _populationInitializer
                .CreateInitialPopulation(missionList, uavList)
                .ToList();

            _logger.LogInformation($"Initial population created with {population.Count} chromosomes");

            EvaluateFitnessParallel(population);

            AssignmentChromosome bestChromosome = GetChromosomeWithBestFitnessScore(population);
            int stagnationCounter = 0;

            _logger.LogInformation(
                $"Initial best fitness: {bestChromosome.FitnessScore:F2}, Assignments: {bestChromosome.Assignments.Count()}"
            );
            _logger.LogInformation("========================================");

            for (
                int generation = 0;
                generation < _algorithmConfiguration.MaxGenerations;
                generation++
            )
            {
                population = CreateNextGeneration(population, missionList, uavList);

                EvaluateFitnessParallel(population);

                AssignmentChromosome currentBestChromosome = GetChromosomeWithBestFitnessScore(
                    population
                );

                if (IsAssignmentBetter(currentBestChromosome, bestChromosome))
                {
                    _logger.LogInformation(
                        $"Generation {generation}: NEW BEST - Fitness: {currentBestChromosome.FitnessScore:F2}, " +
                        $"Assignments: {currentBestChromosome.Assignments.Count()}, " +
                        $"Unique Missions: {currentBestChromosome.Assignments.Select(a => a.Mission.Id).Distinct().Count()}, " +
                        $"Valid: {currentBestChromosome.IsValid}"
                    );
                    bestChromosome = currentBestChromosome;
                    stagnationCounter = 0;
                }
                else
                {
                    stagnationCounter++;
                    if (generation % 10 == 0) // Log every 10 generations
                    {
                        _logger.LogInformation(
                            $"Generation {generation}: Stagnation {stagnationCounter}/{_algorithmConfiguration.StagnationLimit}, " +
                            $"Best Fitness: {bestChromosome.FitnessScore:F2}"
                        );
                    }
                }

                if (ShouldTerminateAlgorithm(stagnationCounter))
                {
                    _logger.LogInformation(
                        $"Algorithm terminated at generation {generation} due to stagnation"
                    );
                    break;
                }
            }

            _logger.LogInformation("========================================");
            _logger.LogInformation("Algorithm Complete");
            _logger.LogInformation($"Final Best Fitness: {bestChromosome.FitnessScore:F2}");
            _logger.LogInformation($"Total Assignments: {bestChromosome.Assignments.Count()}");
            _logger.LogInformation(
                $"Unique Missions Assigned: {bestChromosome.Assignments.Select(a => a.Mission.Id).Distinct().Count()}/{missionList.Count}"
            );
            _logger.LogInformation($"Valid Solution: {bestChromosome.IsValid}");
            _logger.LogInformation("========================================");

            return new AssignmentResult(new[] { bestChromosome });
        }

        private void EvaluateFitnessParallel(List<AssignmentChromosome> population)
        {
            Parallel.ForEach(
                population,
                chromosome =>
                {
                    _fitnessCalculator.CalculateFitness(chromosome);
                }
            );
        }

        private AssignmentChromosome GetChromosomeWithBestFitnessScore(
            List<AssignmentChromosome> population
        )
        {
            return population.OrderByDescending(chromosome => chromosome.FitnessScore).First();
        }

        private bool IsAssignmentBetter(
            AssignmentChromosome newAssignment,
            AssignmentChromosome currentBestAssignment
        )
        {
            return newAssignment.FitnessScore - currentBestAssignment.FitnessScore
                > MissionServiceConstants.MainAlgorithm.NO_IMPROVEMENT_THRESHOLD;
        }

        private bool ShouldTerminateAlgorithm(int stagnationCounter)
        {
            return stagnationCounter >= _algorithmConfiguration.StagnationLimit;
        }

        private List<AssignmentChromosome> CreateNextGeneration(
            List<AssignmentChromosome> currentPopulation,
            List<Mission> missions,
            List<UAV> uavs
        )
        {
            List<AssignmentChromosome> elite = SelectEliteChromosomes(currentPopulation);
            int offspringCount = _algorithmConfiguration.PopulationSize - elite.Count;
            List<AssignmentChromosome> offspringPopulation = CreateOffspringParallel(
                currentPopulation,
                offspringCount,
                uavs
            );

            RepairPopulationParallel(offspringPopulation, missions, uavs);

            List<AssignmentChromosome> validOffspring = offspringPopulation
                .Where(c => c.IsValid)
                .ToList();

            List<AssignmentChromosome> invalidOffspring = offspringPopulation
                .Where(c => !c.IsValid)
                .OrderByDescending(c => c.Assignments.Count()) 
                .ThenByDescending(c => c.FitnessScore) 
                .ToList();

            List<AssignmentChromosome> allOffspring = new List<AssignmentChromosome>();
            allOffspring.AddRange(validOffspring);

            int remainingSlots = offspringCount - validOffspring.Count;
            if (remainingSlots > 0 && invalidOffspring.Any())
            {
                allOffspring.AddRange(invalidOffspring.Take(remainingSlots));
            }

            List<AssignmentChromosome> newPopulation = new List<AssignmentChromosome>(
                _algorithmConfiguration.PopulationSize
            );
            newPopulation.AddRange(elite);
            newPopulation.AddRange(allOffspring);

            return newPopulation;
        }

        private List<AssignmentChromosome> SelectEliteChromosomes(
            List<AssignmentChromosome> population
        )
        {
            int eliteCount = (int)(
                _algorithmConfiguration.PopulationSize * _algorithmConfiguration.ElitePrecentage
            );

            return population
                .OrderByDescending(chromosome => chromosome.FitnessScore)
                .Take(eliteCount)
                .ToList();
        }

        private List<AssignmentChromosome> CreateOffspringParallel(
            List<AssignmentChromosome> population,
            int offspringCount,
            List<UAV> uavs
        )
        {
            int pairCount =
                (offspringCount + 1) / MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE;
            AssignmentChromosome[] offspringPopulation = new AssignmentChromosome[offspringCount];

            Parallel.For(
                0,
                pairCount,
                pairIndex =>
                {
                    AssignmentChromosome parent1 = _selectionStrategy.selectParentChromosome(
                        population
                    );
                    AssignmentChromosome parent2 = _selectionStrategy.selectParentChromosome(
                        population
                    );

                    CrossoverResult crossoverResult = ApplyCrossover(parent1, parent2);

                    ApplyMutation(crossoverResult.FirstChromosome, uavs);
                    ApplyMutation(crossoverResult.SecondChromosome, uavs);

                    int firstOffspringIndex =
                        pairIndex * MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE
                        + MissionServiceConstants.MainAlgorithm.FIRST_OFFSPRING_INDEX;
                    int secondOffspringIndex =
                        pairIndex * MissionServiceConstants.MainAlgorithm.OFFSPRING_PAIR_SIZE
                        + MissionServiceConstants.MainAlgorithm.SECOND_OFFSPRING_INDEX;

                    offspringPopulation[firstOffspringIndex] = crossoverResult.FirstChromosome;

                    if (secondOffspringIndex < offspringCount)
                    {
                        offspringPopulation[secondOffspringIndex] =
                            crossoverResult.SecondChromosome;
                    }
                }
            );

            return offspringPopulation.Where(chromosome => chromosome != null).ToList();
        }

        private CrossoverResult ApplyCrossover(
            AssignmentChromosome parent1,
            AssignmentChromosome parent2
        )
        {
            if (Random.Shared.NextDouble() < _algorithmConfiguration.CrossoverProbability)
            {
                return _crossoverStrategy.CrossoverChromosomes(parent1, parent2);
            }

            return new CrossoverResult { FirstChromosome = parent1, SecondChromosome = parent2 };
        }

        private void ApplyMutation(AssignmentChromosome chromosome, List<UAV> uavs)
        {
            if (Random.Shared.NextDouble() < _algorithmConfiguration.MutationProbability)
            {
                _mutationStrategy.MutateChromosome(chromosome, uavs);
            }
        }

        private void RepairPopulationParallel(
            List<AssignmentChromosome> population,
            List<Mission> missions,
            List<UAV> uavs
        )
        {
            Parallel.ForEach(
                population,
                chromosome =>
                {
                    _repairPipeline.RepairChromosomeViolaitions(chromosome, missions, uavs);
                }
            );
        }
    }
}
