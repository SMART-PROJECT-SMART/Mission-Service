using BenchmarkDotNet.Attributes;
using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Evolution;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Reproduction;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite.Interfaces;
using Mission_Service.Services.UAVStatusService;
using Mission.Service.Benchmarks.Configuration;
using Mission.Service.Benchmarks.Helpers;
using MissionModel = Mission_Service.Models.Mission;

namespace Mission.Service.Benchmarks.Benchmarks
{
    [Config(typeof(SmartBenchmarkConfig))]
    [MemoryDiagnoser]
    [RankColumn]
    public class AlgorithmBenchmarks
    {
        private GeneticAssignmentAlgorithm _algorithm;
        private List<MissionModel> _missions;
        private List<UAV> _uavs;

        [Params(10, 25, 50)]
        public int MissionCount { get; set; }

        [Params(5, 10, 20)]
        public int UAVCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _missions = TestDataGenerator.GenerateMissions(MissionCount);
            _uavs = TestDataGenerator.GenerateUAVs(UAVCount);

            _algorithm = CreateAlgorithm();
        }

        [Benchmark(Description = "Complete Algorithm - Full Execution")]
        [BenchmarkCategory("Algorithm", "Complete")]
        public AssignmentResult CompleteAlgorithm()
        {
            return _algorithm.PreformAssignmentAlgorithm(_missions, _uavs);
        }

        private GeneticAssignmentAlgorithm CreateAlgorithm()
        {
            var algorithmConfig = Options.Create(
                new AssignmentAlgorithmConfiguration
                {
                    PopulationSize = 100,
                    MaxGenerations = 100,
                    ElitePrecentage = 0.1,
                    StagnationLimit = 20,
                    CrossoverProbability = 0.7,
                    MutationProbability = 0.3,
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
                                { TelemetryFields.CurrentSpeedKmph, 30.0 },
                                { TelemetryFields.ThrustAfterInfluence, 25.0 },
                                { TelemetryFields.Altitude, 20.0 },
                            }
                        },
                        {
                            UAVType.Armed,
                            new Dictionary<TelemetryFields, double>
                            {
                                { TelemetryFields.FuelAmount, 35.0 },
                                { TelemetryFields.ThrustAfterInfluence, 45.0 },
                                { TelemetryFields.CurrentSpeedKmph, 40.0 },
                                { TelemetryFields.SignalStrength, 30.0 },
                                { TelemetryFields.FlightTimeSec, 25.0 },
                                { TelemetryFields.Altitude, 20.0 },
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
            var fitnessCalculator = new FitnessCalculator(telemetryWeights, fitnessWeights, uavStatusService);
            var populationInitializer = new PopulationInitializer(algorithmConfig);
            var parallelExecutor = new ParallelExecutor();

            var repairStrategies = new List<IRepairStrategy>
            {
                new TypeMismatchRepairStrategy(),
                new TimeWindowRepairStrategy(),
                new OverlapRepairStrategy(),
                new DuplicateMissionRepairStrategy(),
            };
            var repairPipeline = new RepairPipline(repairStrategies);

            var crossoverStrategy = new UniformCrossoverStrategy();
            var mutationStrategy = new SwapMutationStrategy();
            var selectionStrategy = new TournamentSelectionStrategy(algorithmConfig);
            IEliteSelector eliteSelector = new EliteSelector();

            var offspringGenerator = new OffspringGenerator(
                selectionStrategy,
                crossoverStrategy,
                mutationStrategy,
                algorithmConfig
            );

            var evolutionStrategy = new StandardEvolutionStrategy(
                eliteSelector,
                offspringGenerator,
                parallelExecutor,
                repairPipeline,
                algorithmConfig
            );

            return new GeneticAssignmentAlgorithm(
                fitnessCalculator,
                populationInitializer,
                evolutionStrategy,
                parallelExecutor,
                repairPipeline,
                algorithmConfig
            );
        }
    }
}
