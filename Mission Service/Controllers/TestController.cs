using Core.Common.Enums;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm.Interfaces;

namespace Mission_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IAssignmentAlgorithm _assignmentAlgorithm;
        private readonly ILogger<TestController> _logger;

        public TestController(
            IAssignmentAlgorithm assignmentAlgorithm,
            ILogger<TestController> logger
        )
        {
            _assignmentAlgorithm = assignmentAlgorithm;
            _logger = logger;
        }

        [HttpGet("test/run-all")]
        public IActionResult RunAllTestCases()
        {
            _logger.LogInformation("========================================");
            _logger.LogInformation("RUNNING ALL TEST CASES");
            _logger.LogInformation("========================================");

            var results = new List<object>();

            results.Add(
                ExecuteTestCaseInternal(
                    "Test 1: Basic Equal Assignment",
                    CreateBasicEqualAssignmentData()
                )
            );
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 2: Resource Constrained",
                    CreateResourceConstrainedData()
                )
            );
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 3: Time Overlap Conflict",
                    CreateTimeOverlapConflictData()
                )
            );
            results.Add(ExecuteTestCaseInternal("Test 4: Type Mismatch", CreateTypeMismatchData()));
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 5: Priority Assignment",
                    CreatePriorityAssignmentData()
                )
            );
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 6: Telemetry Optimization",
                    CreateTelemetryOptimizationData()
                )
            );
            results.Add(ExecuteTestCaseInternal("Test 7: Mixed Types", CreateMixedTypesData()));
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 8: Sequential Missions",
                    CreateSequentialMissionsData()
                )
            );
            results.Add(ExecuteTestCaseInternal("Test 9: Stress Test", CreateStressTestData()));
            results.Add(ExecuteTestCaseInternal("Test 10: Edge Cases", CreateEdgeCasesData()));
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 11: UAV Quality Selection",
                    CreateUAVQualitySelectionData()
                )
            );
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 12: Priority vs Telemetry",
                    CreatePriorityVsTelemetryData()
                )
            );
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 13: Parallel Missions Optimization",
                    CreateParallelMissionsOptimizationData()
                )
            );
            results.Add(
                ExecuteTestCaseInternal(
                    "Test 14: Best UAV Per Mission Type",
                    CreateBestUAVPerMissionTypeData()
                )
            );
            results.Add(
                ExecuteTestCaseInternal("Test 15: Maximum Coverage", CreateMaximumCoverageData())
            );

            _logger.LogInformation("========================================");
            _logger.LogInformation("ALL TEST CASES COMPLETED");
            _logger.LogInformation("========================================");

            return Ok(new { TestResults = results });
        }

        [HttpGet("test/basic-equal-assignment")]
        public IActionResult TestBasicEqualAssignment()
        {
            var (missions, uavs) = CreateBasicEqualAssignmentData();
            return ExecuteTestCase("Basic Equal Assignment", missions, uavs);
        }

        [HttpGet("test/resource-constrained")]
        public IActionResult TestResourceConstrained()
        {
            var (missions, uavs) = CreateResourceConstrainedData();
            return ExecuteTestCase("Resource Constrained", missions, uavs);
        }

        [HttpGet("test/time-overlap-conflict")]
        public IActionResult TestTimeOverlapConflict()
        {
            var (missions, uavs) = CreateTimeOverlapConflictData();
            return ExecuteTestCase("Time Overlap Conflict", missions, uavs);
        }

        [HttpGet("test/type-mismatch")]
        public IActionResult TestTypeMismatch()
        {
            var (missions, uavs) = CreateTypeMismatchData();
            return ExecuteTestCase("Type Mismatch", missions, uavs);
        }

        [HttpGet("test/priority-assignment")]
        public IActionResult TestPriorityAssignment()
        {
            var (missions, uavs) = CreatePriorityAssignmentData();
            return ExecuteTestCase("Priority-Based Assignment", missions, uavs);
        }

        [HttpGet("test/telemetry-optimization")]
        public IActionResult TestTelemetryOptimization()
        {
            var (missions, uavs) = CreateTelemetryOptimizationData();
            return ExecuteTestCase("Telemetry Optimization", missions, uavs);
        }

        [HttpGet("test/mixed-types")]
        public IActionResult TestMixedTypes()
        {
            var (missions, uavs) = CreateMixedTypesData();
            return ExecuteTestCase("Mixed Types", missions, uavs);
        }

        [HttpGet("test/sequential-missions")]
        public IActionResult TestSequentialMissions()
        {
            var (missions, uavs) = CreateSequentialMissionsData();
            return ExecuteTestCase("Sequential Missions", missions, uavs);
        }

        [HttpGet("test/stress-test")]
        public IActionResult TestStressTest()
        {
            var (missions, uavs) = CreateStressTestData();
            return ExecuteTestCase("Stress Test", missions, uavs);
        }

        [HttpGet("test/edge-cases")]
        public IActionResult TestEdgeCases()
        {
            var (missions, uavs) = CreateEdgeCasesData();
            return ExecuteTestCase("Edge Cases", missions, uavs);
        }

        [HttpGet("test/uav-quality-selection")]
        public IActionResult TestUAVQualitySelection()
        {
            var (missions, uavs) = CreateUAVQualitySelectionData();
            return ExecuteTestCase("UAV Quality Selection", missions, uavs);
        }

        [HttpGet("test/priority-vs-telemetry")]
        public IActionResult TestPriorityVsTelemetry()
        {
            var (missions, uavs) = CreatePriorityVsTelemetryData();
            return ExecuteTestCase("Priority vs Telemetry", missions, uavs);
        }

        [HttpGet("test/parallel-missions-optimization")]
        public IActionResult TestParallelMissionsOptimization()
        {
            var (missions, uavs) = CreateParallelMissionsOptimizationData();
            return ExecuteTestCase("Parallel Missions Optimization", missions, uavs);
        }

        [HttpGet("test/best-uav-per-mission-type")]
        public IActionResult TestBestUAVPerMissionType()
        {
            var (missions, uavs) = CreateBestUAVPerMissionTypeData();
            return ExecuteTestCase("Best UAV Per Mission Type", missions, uavs);
        }

        [HttpGet("test/maximum-coverage")]
        public IActionResult TestMaximumCoverage()
        {
            var (missions, uavs) = CreateMaximumCoverageData();
            return ExecuteTestCase("Maximum Coverage", missions, uavs);
        }

        private IActionResult ExecuteTestCase(
            string testName,
            List<Mission> missions,
            List<UAV> uavs
        )
        {
            var result = ExecuteTestCaseInternal(testName, (missions, uavs));
            return Ok(result);
        }

        private object ExecuteTestCaseInternal(
            string testName,
            (List<Mission> missions, List<UAV> uavs) testData
        )
        {
            try
            {
                var (missions, uavs) = testData;
                var startTime = DateTime.Now;

                _logger.LogInformation("========================================");
                _logger.LogInformation($"TEST: {testName}");
                _logger.LogInformation("========================================");

                _logger.LogInformation($"MISSIONS ({missions.Count}):");
                foreach (var mission in missions)
                {
                    _logger.LogInformation(
                        $"  ID: {mission.Id}, Type: {mission.RequiredUAVType}, Priority: {mission.Priority}, "
                            + $"TimeWindow: {mission.TimeWindow.Start:HH:mm}-{mission.TimeWindow.End:HH:mm}, "
                            + $"Location: ({mission.Location.Latitude:F4}, {mission.Location.Longitude:F4}, {mission.Location.Altitude:F0})"
                    );
                }

                _logger.LogInformation($"UAVS ({uavs.Count}):");
                foreach (var uav in uavs)
                {
                    _logger.LogInformation($"  TailID: {uav.TailId}, Type: {uav.UavType}");
                    foreach (var telemetry in uav.TelemetryData)
                    {
                        _logger.LogInformation($"    {telemetry.Key}: {telemetry.Value:F2}");
                    }
                }

                _logger.LogInformation("EXECUTING ALGORITHM...");
                var result = _assignmentAlgorithm.PreformAssignmentAlgorithm(missions, uavs);

                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                _logger.LogInformation($"EXECUTION TIME: {duration.TotalMilliseconds}ms");
                _logger.LogInformation($"CHROMOSOMES GENERATED: {result.Assignments.Count()}");

                foreach (var chromosome in result.Assignments)
                {
                    _logger.LogInformation(
                        $"CHROMOSOME - Fitness: {chromosome.FitnessScore:F6}, Valid: {chromosome.IsValid}, Assignments: {chromosome.Assignments.Count()}"
                    );

                    foreach (var gene in chromosome.Assignments)
                    {
                        _logger.LogInformation(
                            $"  Mission: {gene.Mission.Id} -> UAV: {gene.UAV.TailId} (Type: {gene.UAV.UavType}), "
                                + $"Start: {gene.StartTime:yyyy-MM-dd HH:mm:ss}, Duration: {gene.Duration}, End: {gene.EndTime:yyyy-MM-dd HH:mm:ss}"
                        );
                    }
                }

                _logger.LogInformation("========================================");

                return new
                {
                    TestName = testName,
                    ExecutionTimeMs = duration.TotalMilliseconds,
                    Input = new
                    {
                        Missions = missions.Select(m => new
                        {
                            m.Id,
                            RequiredType = m.RequiredUAVType.ToString(),
                            Priority = m.Priority.ToString(),
                            TimeWindow = new { m.TimeWindow.Start, m.TimeWindow.End },
                            m.Location,
                        }),
                        UAVs = uavs.Select(u => new
                        {
                            u.TailId,
                            Type = u.UavType.ToString(),
                            u.TelemetryData,
                        }),
                    },
                    Results = new
                    {
                        ChromosomesGenerated = result.Assignments.Count(),
                        BestFitnessScore = result.Assignments.FirstOrDefault()?.FitnessScore ?? 0,
                        Assignments = result.Assignments.Select(c => new
                        {
                            c.FitnessScore,
                            c.IsValid,
                            AssignmentCount = c.Assignments.Count(),
                            Details = c.Assignments.Select(g => new
                            {
                                MissionId = g.Mission.Id,
                                UAVTailId = g.UAV.TailId,
                                UAVType = g.UAV.UavType.ToString(),
                                g.StartTime,
                                g.Duration,
                                g.EndTime,
                            }),
                        }),
                    },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR IN TEST: {testName}");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                return new
                {
                    TestName = testName,
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                };
            }
        }

        private (List<Mission>, List<UAV>) CreateBasicEqualAssignmentData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(5)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(2), DateTime.Now.AddHours(6)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1.5),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(102, UAVType.Armed, CreateOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateResourceConstrainedData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(4), DateTime.Now.AddHours(6)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Low,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(7), DateTime.Now.AddHours(9)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M4",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(2), DateTime.Now.AddHours(5)),
                    Location = new Location(31.2530, 34.7915, 120),
                    Duration = TimeSpan.FromHours(1.5),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(102, UAVType.Armed, CreateSubOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateTimeOverlapConflictData()
        {
            DateTime baseTime = DateTime.Now.AddHours(1);
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(3)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime.AddHours(2), baseTime.AddHours(5)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(baseTime.AddHours(4), baseTime.AddHours(7)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromHours(1.5),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateTypeMismatchData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(5), DateTime.Now.AddHours(8)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1.5),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(102, UAVType.Surveillance, CreateOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreatePriorityAssignmentData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Low,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromHours(1.5),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateTelemetryOptimizationData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(4)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1.5),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(102, UAVType.Surveillance, CreateSubOptimalTelemetry()),
                new UAV(103, UAVType.Surveillance, CreatePoorTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateMixedTypesData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(2), DateTime.Now.AddHours(5)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(4), DateTime.Now.AddHours(7)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromHours(1.5),
                },
                new Mission
                {
                    Id = "M4",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.Low,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(6), DateTime.Now.AddHours(9)),
                    Location = new Location(31.2530, 34.7915, 120),
                    Duration = TimeSpan.FromHours(1.5),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(102, UAVType.Armed, CreateOptimalTelemetry()),
                new UAV(103, UAVType.Surveillance, CreateSubOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateSequentialMissionsData()
        {
            DateTime baseTime = DateTime.Now.AddHours(1);
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(2)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime.AddHours(3), baseTime.AddHours(5)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime.AddHours(6), baseTime.AddHours(8)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromHours(1),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateStressTestData()
        {
            List<Mission> missions = new List<Mission>();
            DateTime baseTime = DateTime.Now.AddHours(1);

            for (int i = 1; i <= 20; i++)
            {
                missions.Add(
                    new Mission
                    {
                        Id = $"M{i}",
                        RequiredUAVType = i % 2 == 0 ? UAVType.Armed : UAVType.Surveillance,
                        Priority = (MissionPriority)(i % 3),
                        TimeWindow = new TimeWindow(
                            baseTime.AddHours(i * 0.5),
                            baseTime.AddHours(i * 0.5 + 2)
                        ),
                        Location = new Location(32.0 + (i * 0.1), 34.7 + (i * 0.1), 100 + (i * 10)),
                        Duration = TimeSpan.FromMinutes(45 + (i % 3) * 15),
                    }
                );
            }

            List<UAV> uavs = new List<UAV>();
            for (int i = 1; i <= 10; i++)
            {
                uavs.Add(
                    new UAV(
                        100 + i,
                        i % 2 == 0 ? UAVType.Armed : UAVType.Surveillance,
                        i % 3 == 0 ? CreatePoorTelemetry()
                            : i % 3 == 1 ? CreateOptimalTelemetry()
                            : CreateSubOptimalTelemetry()
                    )
                );
            }

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateEdgeCasesData()
        {
            return (new List<Mission>(), new List<UAV>());
        }

        private (List<Mission>, List<UAV>) CreateUAVQualitySelectionData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(4), DateTime.Now.AddHours(6)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(101, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(102, UAVType.Surveillance, CreateSubOptimalTelemetry()),
                new UAV(103, UAVType.Surveillance, CreatePoorTelemetry()),
                new UAV(104, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(105, UAVType.Surveillance, CreateSubOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreatePriorityVsTelemetryData()
        {
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.Low,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(DateTime.Now.AddHours(1), DateTime.Now.AddHours(3)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(201, UAVType.Armed, CreateOptimalTelemetry()),
                new UAV(202, UAVType.Armed, CreatePoorTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateParallelMissionsOptimizationData()
        {
            DateTime baseTime = DateTime.Now.AddHours(1);
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(2)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(2)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(2)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromHours(1),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(301, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(302, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(303, UAVType.Surveillance, CreateSubOptimalTelemetry()),
                new UAV(304, UAVType.Surveillance, CreatePoorTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateBestUAVPerMissionTypeData()
        {
            DateTime baseTime = DateTime.Now.AddHours(1);
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(2)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(2)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime.AddHours(3), baseTime.AddHours(5)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M4",
                    RequiredUAVType = UAVType.Armed,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime.AddHours(3), baseTime.AddHours(5)),
                    Location = new Location(31.2530, 34.7915, 120),
                    Duration = TimeSpan.FromHours(1),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(401, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(402, UAVType.Surveillance, CreateSubOptimalTelemetry()),
                new UAV(403, UAVType.Surveillance, CreatePoorTelemetry()),
                new UAV(404, UAVType.Armed, CreateOptimalTelemetry()),
                new UAV(405, UAVType.Armed, CreateSubOptimalTelemetry()),
                new UAV(406, UAVType.Armed, CreatePoorTelemetry()),
            };

            return (missions, uavs);
        }

        private (List<Mission>, List<UAV>) CreateMaximumCoverageData()
        {
            DateTime baseTime = DateTime.Now.AddHours(1);
            List<Mission> missions = new List<Mission>
            {
                new Mission
                {
                    Id = "M1",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime, baseTime.AddHours(2)),
                    Location = new Location(32.0853, 34.7818, 100),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M2",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(baseTime.AddHours(1), baseTime.AddHours(3)),
                    Location = new Location(31.7683, 35.2137, 150),
                    Duration = TimeSpan.FromMinutes(45),
                },
                new Mission
                {
                    Id = "M3",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Low,
                    TimeWindow = new TimeWindow(baseTime.AddHours(2), baseTime.AddHours(4)),
                    Location = new Location(32.4427, 34.9235, 200),
                    Duration = TimeSpan.FromMinutes(45),
                },
                new Mission
                {
                    Id = "M4",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.Medium,
                    TimeWindow = new TimeWindow(baseTime.AddHours(3), baseTime.AddHours(5)),
                    Location = new Location(31.2530, 34.7915, 120),
                    Duration = TimeSpan.FromHours(1),
                },
                new Mission
                {
                    Id = "M5",
                    RequiredUAVType = UAVType.Surveillance,
                    Priority = MissionPriority.High,
                    TimeWindow = new TimeWindow(baseTime.AddHours(4), baseTime.AddHours(6)),
                    Location = new Location(32.1500, 34.8500, 130),
                    Duration = TimeSpan.FromHours(1),
                },
            };

            List<UAV> uavs = new List<UAV>
            {
                new UAV(501, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(502, UAVType.Surveillance, CreateOptimalTelemetry()),
                new UAV(503, UAVType.Surveillance, CreateSubOptimalTelemetry()),
            };

            return (missions, uavs);
        }

        private Dictionary<TelemetryFields, double> CreateOptimalTelemetry()
        {
            return new Dictionary<TelemetryFields, double>
            {
                { TelemetryFields.FuelAmount, 95.0 },
                { TelemetryFields.SignalStrength, 90.0 },
                { TelemetryFields.DataStorageUsedGB, 20.0 },
                { TelemetryFields.FlightTimeSec, 50000.0 },
                { TelemetryFields.CurrentSpeedKmph, 150.0 },
                { TelemetryFields.Altitude, 1000.0 },
                { TelemetryFields.ThrottlePercent, 75.0 },
                { TelemetryFields.ThrustAfterInfluence, 80000.0 },
                { TelemetryFields.Rpm, 5000.0 },
            };
        }

        private Dictionary<TelemetryFields, double> CreateSubOptimalTelemetry()
        {
            return new Dictionary<TelemetryFields, double>
            {
                { TelemetryFields.FuelAmount, 60.0 },
                { TelemetryFields.SignalStrength, 70.0 },
                { TelemetryFields.DataStorageUsedGB, 50.0 },
                { TelemetryFields.FlightTimeSec, 30000.0 },
                { TelemetryFields.CurrentSpeedKmph, 120.0 },
                { TelemetryFields.Altitude, 800.0 },
                { TelemetryFields.ThrottlePercent, 60.0 },
                { TelemetryFields.ThrustAfterInfluence, 60000.0 },
                { TelemetryFields.Rpm, 4000.0 },
            };
        }

        private Dictionary<TelemetryFields, double> CreatePoorTelemetry()
        {
            return new Dictionary<TelemetryFields, double>
            {
                { TelemetryFields.FuelAmount, 30.0 },
                { TelemetryFields.SignalStrength, 40.0 },
                { TelemetryFields.DataStorageUsedGB, 80.0 },
                { TelemetryFields.FlightTimeSec, 10000.0 },
                { TelemetryFields.CurrentSpeedKmph, 80.0 },
                { TelemetryFields.Altitude, 500.0 },
                { TelemetryFields.ThrottlePercent, 40.0 },
                { TelemetryFields.ThrustAfterInfluence, 40000.0 },
                { TelemetryFields.Rpm, 3000.0 },
            };
        }
    }
}
