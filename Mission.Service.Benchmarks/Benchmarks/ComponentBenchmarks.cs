using BenchmarkDotNet.Attributes;
using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies.Interfaces;
using Mission_Service.Services.UAVStatusService;
using Mission.Service.Benchmarks.Configuration;
using Mission.Service.Benchmarks.Helpers;
using MissionModel = Mission_Service.Models.Mission;

namespace Mission.Service.Benchmarks.Benchmarks
{
    [Config(typeof(SmartBenchmarkConfig))]
    [MemoryDiagnoser]
    [RankColumn]
    public class ComponentBenchmarks
    {
        private IFitnessCalculator _fitnessCalculator;
        private ICrossoverStrategy _crossoverStrategy;
        private IMutationStrategy _mutationStrategy;
        private IPopulationInitializer _populationInitializer;
        private IRepairPipeline _repairPipeline;

        private List<MissionModel> _missions;
        private List<UAV> _uavs;
        private AssignmentChromosome _sampleChromosome;

        [Params(10, 25, 50)]
        public int MissionCount { get; set; }

        [Params(5, 15)]
        public int UAVCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _missions = TestDataGenerator.GenerateMissions(MissionCount);
            _uavs = TestDataGenerator.GenerateUAVs(UAVCount);

            InitializeComponents();

            _sampleChromosome = TestDataGenerator.CreateSampleChromosome(_missions, _uavs);
        }

        [Benchmark(Description = "Fitness - Single Chromosome", Baseline = true)]
        [BenchmarkCategory("Component", "Fitness")]
        public double Fitness_Single()
        {
            return _fitnessCalculator.CalculateFitness(_sampleChromosome);
        }

        [Benchmark(Description = "Crossover - Uniform")]
        [BenchmarkCategory("Component", "Crossover")]
        public CrossoverResult Crossover_Uniform()
        {
            return _crossoverStrategy.CrossoverChromosomes(
                _sampleChromosome,
                _sampleChromosome.Clone()
            );
        }

        [Benchmark(Description = "Mutation - Swap")]
        [BenchmarkCategory("Component", "Mutation")]
        public void Mutation_Swap()
        {
            var clone = _sampleChromosome.Clone();
            _mutationStrategy.MutateChromosome(clone, _uavs);
        }

        [Benchmark(Description = "Population - Initialize")]
        [BenchmarkCategory("Component", "Population")]
        public int Population_Initialize()
        {
            return _populationInitializer.CreateInitialPopulation(_missions, _uavs).Count();
        }

        [Benchmark(Description = "Repair - Single Chromosome")]
        [BenchmarkCategory("Component", "Repair")]
        public void Repair_Single()
        {
            var clone = _sampleChromosome.Clone();
            _repairPipeline.RepairChromosomeViolaitions(clone, _missions, _uavs);
        }

        private void InitializeComponents()
        {
            var algorithmConfig = Options.Create(
                new AssignmentAlgorithmConfiguration
                {
                    PopulationSize = 50,
                    MaxGenerations = 100,
                    ElitePrecentage = 0.1,
                    StagnationLimit = 20,
                    CrossoverProbability = 0.8,
                    MutationProbability = 0.1,
                    TournamentSize = 5,
                }
            );

            var telemetryWeights = Options.Create(
                new TelemetryWeightsConfiguration
                {
                    Weights = new Dictionary<UAVType, Dictionary<TelemetryFields, double>>
                    {
                        {
                            UAVType.Surveillance,
                            new Dictionary<TelemetryFields, double>
                            {
                                { TelemetryFields.FuelAmount, 35.0 },
                                { TelemetryFields.SignalStrength, 45.0 },
                                { TelemetryFields.FlightTimeSec, 40.0 },
                            }
                        },
                        {
                            UAVType.Armed,
                            new Dictionary<TelemetryFields, double>
                            {
                                { TelemetryFields.FuelAmount, 35.0 },
                                { TelemetryFields.ThrustAfterInfluence, 45.0 },
                                { TelemetryFields.CurrentSpeedKmph, 40.0 },
                            }
                        },
                    },
                }
            );

            var fitnessWeights = Options.Create(
                new FitnessWeightsConfiguration
                {
                    TelemetryOptimization = 150.0,
                    PriorityCoverage = 100.0,
                    MissionCoverageWeight = 1000.0,
                    TimeOverlapPenalty = -10000.0,
                    TypeMismatchPenalty = -10000.0,
                    ActiveMissionPenalty = -500.0,
                }
            );

            var uavStatusService = new UAVStatus();

            _fitnessCalculator = new FitnessCalculator(telemetryWeights, fitnessWeights, uavStatusService);
            _crossoverStrategy = new UniformCrossoverStrategy();
            _mutationStrategy = new SwapMutationStrategy();
            _populationInitializer = new PopulationInitializer(algorithmConfig);

            var repairStrategies = new List<IRepairStrategy>
            {
                new TimeWindowRepairStrategy(),
                new TypeMismatchRepairStrategy(),
                new OverlapRepairStrategy(),
                new DuplicateMissionRepairStrategy(),
            };
            _repairPipeline = new RepairPipline(repairStrategies);
        }
    }
}
