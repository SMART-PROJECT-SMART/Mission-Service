using Core.Common.Enums;
using Microsoft.Extensions.Options;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Reproduction;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite;
using Xunit;

namespace Mission_Service.Tests;

public class GeneticAssignmentAlgorithmTests
{
    private readonly GeneticAssignmentAlgorithm _algorithm;
    private readonly AssignmentAlgorithmConfiguration _config;

    public GeneticAssignmentAlgorithmTests()
    {
        _config = new AssignmentAlgorithmConfiguration
        {
            PopulationSize = 20,
            MaxGenerations = 50,
            CrossoverProbability = 0.7,
            MutationProbability = 0.3,
            ElitePrecentage = 0.1,
            TournamentSize = 3,
            StagnationLimit = 10,
        };

        var telemetryWeights = new TelemetryWeightsConfiguration
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
        };

        var fitnessWeights = new FitnessWeightsConfiguration
        {
            TelemetryOptimization = 150.0,
            PriorityCoverage = 100.0,
            TimeOverlapPenalty = -10000.0,
            TypeMismatchPenalty = -10000.0,
            MissionCoverageWeight = 1000.0,
        };

        var fitnessCalculator = new FitnessCalculator(
            Options.Create(telemetryWeights),
            Options.Create(fitnessWeights)
        );

        var populationInitializer = new PopulationInitializer(Options.Create(_config));
        var selectionStrategy = new TournamentSelectionStrategy(Options.Create(_config));
        var crossoverStrategy = new UniformCrossoverStrategy();
        var mutationStrategy = new SwapMutationStrategy();
        var eliteSelector = new EliteSelector();
        var offspringGenerator = new OffspringGenerator(
            selectionStrategy,
            crossoverStrategy,
            mutationStrategy,
            Options.Create(_config)
        );
        var parallelExecutor = new ParallelExecutor();

        var repairStrategies = new List<IRepairStrategy>
        {
            new TypeMismatchRepairStrategy(),
            new TimeWindowRepairStrategy(),
            new OverlapRepairStrategy(),
            new DuplicateMissionRepairStrategy(),
        };
        var repairPipeline = new RepairPipline(repairStrategies);

        _algorithm = new GeneticAssignmentAlgorithm(
            fitnessCalculator,
            populationInitializer,
            eliteSelector,
            offspringGenerator,
            parallelExecutor,
            repairPipeline,
            Options.Create(_config)
        );
    }

    [Fact]
    public void PerformAlgorithm_ValidInput_ReturnsResult()
    {
        // Arrange
        var (missions, uavs) = CreateBasicTestData();

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Assignments);
        Assert.NotEmpty(result.Assignments);
    }

    [Fact]
    public void PerformAlgorithm_ValidInput_ReturnsValidAssignment()
    {
        // Arrange
        var (missions, uavs) = CreateBasicTestData();

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);
        var bestChromosome = result.Assignments.First();

        // Assert
        Assert.True(bestChromosome.IsValid);
        Assert.True(bestChromosome.FitnessScore > 0);
    }

    [Fact]
    public void PerformAlgorithm_BasicScenario_AssignsAllMissions()
    {
        // Arrange
        var (missions, uavs) = CreateBasicTestData();

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);
        var bestChromosome = result.Assignments.First();

        // Assert
        var assignedMissionIds = bestChromosome.Assignments.Select(a => a.Mission.Id).ToHashSet();
        Assert.Equal(missions.Count, assignedMissionIds.Count);
    }

    [Fact]
    public void PerformAlgorithm_HighPriorityMission_GetsAssigned()
    {
        // Arrange
        var missions = new List<Mission>
        {
            new Mission
            {
                Id = "M1",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddHours(2)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1),
            },
            new Mission
            {
                Id = "M2",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.Low,
                TimeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddHours(2)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1),
            },
        };

        var uavs = new List<UAV> { new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()) };

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);
        var bestChromosome = result.Assignments.First();

        // Assert - High priority should be assigned
        var assignedMissionIds = bestChromosome.Assignments.Select(a => a.Mission.Id).ToList();
        Assert.Contains("M1", assignedMissionIds);
    }

    [Fact]
    public void PerformAlgorithm_TypeMismatch_CorrectlyHandles()
    {
        // Arrange - Armed missions but only Surveillance UAVs
        var missions = new List<Mission>
        {
            new Mission
            {
                Id = "M1",
                RequiredUAVType = UAVType.Armed,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddHours(2)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1),
            },
        };

        var uavs = new List<UAV> { new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()) };

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);
        var bestChromosome = result.Assignments.First();

        // Assert - Should handle gracefully (either no assignment or marked invalid)
        Assert.NotNull(bestChromosome);
    }

    [Fact]
    public void PerformAlgorithm_OverlappingTimeWindows_NoOverlaps()
    {
        // Arrange
        DateTime baseTime = DateTime.Now;
        List<Mission> missions = new List<Mission>
        {
            new Mission
            {
                Id = "M1",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(4)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1.5),
            },
            new Mission
            {
                Id = "M2",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(baseTime.AddHours(1), baseTime.AddHours(5)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1.5),
            },
        };

        List<UAV> uavs = new List<UAV> { new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()) };

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);
        var bestChromosome = result.Assignments.First();

        // Assert - Check no time overlaps for same UAV
        List<AssignmentGene> uavAssignments = bestChromosome.Assignments.OrderBy(a => a.StartTime).ToList();

        for (int i = 0; i < uavAssignments.Count - 1; i++)
        {
            Assert.True(
                uavAssignments[i].EndTime <= uavAssignments[i + 1].StartTime,
                "Assignments should not overlap"
            );
        }
    }

    [Fact]
    public void PerformAlgorithm_BetterTelemetry_HigherFitness()
    {
        // Arrange
        var missions = new List<Mission>
        {
            new Mission
            {
                Id = "M1",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.Medium,
                TimeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddHours(2)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1),
            },
        };

        var optimalUavs = new List<UAV>
        {
            new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
        };

        var poorUavs = new List<UAV> { new UAV(102, UAVType.Surveillance, CreatePoorTelemetry()) };

        // Act
        var optimalResult = _algorithm.PreformAssignmentAlgorithm(missions, optimalUavs);
        var poorResult = _algorithm.PreformAssignmentAlgorithm(missions, poorUavs);

        // Assert
        Assert.True(
            optimalResult.Assignments.First().FitnessScore
                > poorResult.Assignments.First().FitnessScore
        );
    }

    [Fact]
    public void PerformAlgorithm_EmptyMissions_ReturnsEmptyAssignments()
    {
        // Arrange
        var missions = new List<Mission>();
        var uavs = new List<UAV> { new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()) };

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);
        var bestChromosome = result.Assignments.First();

        // Assert
        Assert.Empty(bestChromosome.Assignments);
    }

    [Fact]
    public void PerformAlgorithm_EmptyUAVs_HandlesGracefully()
    {
        // Arrange
        var missions = new List<Mission>
        {
            new Mission
            {
                Id = "M1",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddHours(2)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1),
            },
        };
        var uavs = new List<UAV>();

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void PerformAlgorithm_MultipleMissionsAndUAVs_OptimizesAssignment()
    {
        // Arrange
        DateTime baseTime = DateTime.Now;
        List<Mission> missions = new List<Mission>
        {
            new Mission
            {
                Id = "M1",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(3)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1.5),
            },
            new Mission
            {
                Id = "M2",
                RequiredUAVType = UAVType.Armed,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(baseTime.AddHours(3), baseTime.AddHours(6)),
                Location = new Core.Models.Location(31.0, 35.0, 150),
                Duration = TimeSpan.FromHours(1),
            },
            new Mission
            {
                Id = "M3",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.Medium,
                TimeWindow = new TimeWindow(baseTime.AddHours(6), baseTime.AddHours(9)),
                Location = new Core.Models.Location(32.5, 34.5, 200),
                Duration = TimeSpan.FromHours(2),
            },
        };

        List<UAV> uavs = new List<UAV>
        {
            new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
            new UAV(102, UAVType.Armed, CreateOptimalTelemetry()),
            new UAV(103, UAVType.Surveillance, CreateSubOptimalTelemetry()),
        };

        // Act
        var result = _algorithm.PreformAssignmentAlgorithm(missions, uavs);
        var bestChromosome = result.Assignments.First();

        // Assert
        Assert.True(bestChromosome.IsValid);
        Assert.True(bestChromosome.FitnessScore > 0);
        Assert.Equal(3, bestChromosome.Assignments.Count());
    }

    // Helper methods
    private (List<Mission>, List<UAV>) CreateBasicTestData()
    {
        List<Mission> missions = new List<Mission>
        {
            new Mission
            {
                Id = "M1",
                RequiredUAVType = UAVType.Surveillance,
                Priority = MissionPriority.High,
                TimeWindow = new TimeWindow(DateTime.Now, DateTime.Now.AddHours(3)),
                Location = new Core.Models.Location(32.0, 34.0, 100),
                Duration = TimeSpan.FromHours(1),
            },
            new Mission
            {
                Id = "M2",
                RequiredUAVType = UAVType.Armed,
                Priority = MissionPriority.Medium,
                TimeWindow = new TimeWindow(DateTime.Now.AddHours(3), DateTime.Now.AddHours(6)),
                Location = new Core.Models.Location(31.0, 35.0, 150),
                Duration = TimeSpan.FromMinutes(45),
            },
        };

        List<UAV> uavs = new List<UAV>
        {
            new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
            new UAV(102, UAVType.Armed, CreateOptimalTelemetry()),
        };

        return (missions, uavs);
    }

    private Dictionary<TelemetryFields, double> CreateOptimalTelemetry()
    {
        return new Dictionary<TelemetryFields, double>
        {
            { TelemetryFields.FuelAmount, 95.0 },
            { TelemetryFields.SignalStrength, 90.0 },
            { TelemetryFields.FlightTimeSec, 50000.0 },
            { TelemetryFields.CurrentSpeedKmph, 150.0 },
            { TelemetryFields.ThrustAfterInfluence, 80000.0 },
        };
    }

    private Dictionary<TelemetryFields, double> CreateSubOptimalTelemetry()
    {
        return new Dictionary<TelemetryFields, double>
        {
            { TelemetryFields.FuelAmount, 60.0 },
            { TelemetryFields.SignalStrength, 70.0 },
            { TelemetryFields.FlightTimeSec, 30000.0 },
            { TelemetryFields.CurrentSpeedKmph, 120.0 },
            { TelemetryFields.ThrustAfterInfluence, 60000.0 },
        };
    }

    private Dictionary<TelemetryFields, double> CreatePoorTelemetry()
    {
        return new Dictionary<TelemetryFields, double>
        {
            { TelemetryFields.FuelAmount, 30.0 },
            { TelemetryFields.SignalStrength, 40.0 },
            { TelemetryFields.FlightTimeSec, 10000.0 },
            { TelemetryFields.CurrentSpeedKmph, 80.0 },
            { TelemetryFields.ThrustAfterInfluence, 40000.0 },
        };
    }
}
