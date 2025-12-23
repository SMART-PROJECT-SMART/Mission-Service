using BenchmarkDotNet.Attributes;
using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator.Interfaces;
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
    [ThreadingDiagnoser]
    [RankColumn]
    public class ParallelBenchmarks
    {
        private IFitnessCalculator _fitnessCalculator;
        private IParallelExecutor _parallelExecutor;
        private IRepairPipeline _repairPipeline;
        private List<AssignmentChromosome> _population;
        private List<MissionModel> _missions;
        private List<UAV> _uavs;

        [Params(50, 100)]
        public int PopulationSize { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _missions = TestDataGenerator.GenerateMissions(25);
            _uavs = TestDataGenerator.GenerateUAVs(15);

            InitializeComponents();

            _population = Enumerable
                .Range(0, PopulationSize)
                .Select(_ => TestDataGenerator.CreateSampleChromosome(_missions, _uavs))
                .ToList();
        }

        [Benchmark(Description = "Fitness - Sequential", Baseline = true)]
        [BenchmarkCategory("Parallel", "Fitness")]
        public void Fitness_Sequential()
        {
            foreach (var chromosome in _population)
            {
                _fitnessCalculator.CalculateFitness(chromosome);
            }
        }

        [Benchmark(Description = "Fitness - Parallel")]
        [BenchmarkCategory("Parallel", "Fitness")]
        public void Fitness_Parallel()
        {
            _parallelExecutor.EvaluatePopulationFitnessInParallel(
                _population,
                chromosome => _fitnessCalculator.CalculateFitness(chromosome)
            );
        }

        [Benchmark(Description = "Repair - Sequential")]
        [BenchmarkCategory("Parallel", "Repair")]
        public void Repair_Sequential()
        {
            var cloned = _population.Select(c => c.Clone()).ToList();
            foreach (var chromosome in cloned)
            {
                _repairPipeline.RepairChromosomeViolaitions(chromosome, _missions, _uavs);
            }
        }

        [Benchmark(Description = "Repair - Parallel")]
        [BenchmarkCategory("Parallel", "Repair")]
        public void Repair_Parallel()
        {
            var cloned = _population.Select(c => c.Clone()).ToList();
            _parallelExecutor.RepairPopulationInParallel(
                cloned,
                chromosome =>
                    _repairPipeline.RepairChromosomeViolaitions(chromosome, _missions, _uavs)
            );
        }

        private void InitializeComponents()
        {
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
            _parallelExecutor = new ParallelExecutor();

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
