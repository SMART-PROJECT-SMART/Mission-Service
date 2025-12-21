using Core.Common.Enums;
using Core.Models;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission.Service.Benchmarks.Helpers
{
    public static class TestDataGenerator
    {
        public static List<Mission_Service.Models.Mission> GenerateMissions(
            int count,
            int seed = 42
        )
        {
            var random = new Random(seed);
            var missions = new List<Mission_Service.Models.Mission>(count);

            for (int i = 0; i < count; i++)
            {
                missions.Add(
                    new Mission_Service.Models.Mission
                    {
                        Id = $"M{i:D4}",
                        Priority = (MissionPriority)(i % 3),
                        RequiredUAVType = (UAVType)(i % 2),
                        TimeWindow = new TimeWindow(
                            DateTime.UtcNow.AddHours(i),
                            DateTime.UtcNow.AddHours(i + 24)
                        ),
                        Location = new Location(
                            32.0 + (random.NextDouble() * 0.1),
                            34.0 + (random.NextDouble() * 0.1),
                            100 + random.Next(0, 200)
                        ),
                    }
                );
            }

            return missions;
        }

        public static List<UAV> GenerateUAVs(int count, int seed = 42)
        {
            var random = new Random(seed);
            var uavs = new List<UAV>(count);

            for (int i = 0; i < count; i++)
            {
                uavs.Add(
                    new UAV
                    {
                        TailId = i,
                        UavType = (UAVType)(i % 2),
                        TelemetryData = new Dictionary<TelemetryFields, double>
                        {
                            [TelemetryFields.FuelAmount] = 50 + random.Next(0, 50),
                            [TelemetryFields.SignalStrength] = 70 + random.Next(0, 30),
                            [TelemetryFields.FlightTimeSec] = 30000 + random.Next(0, 20000),
                            [TelemetryFields.CurrentSpeedKmph] = 100 + random.Next(0, 100),
                            [TelemetryFields.ThrustAfterInfluence] = 50000 + random.Next(0, 30000),
                            [TelemetryFields.Altitude] = 100 + random.Next(0, 200),
                        },
                    }
                );
            }

            return uavs;
        }

        public static AssignmentChromosome CreateSampleChromosome(
            List<Mission_Service.Models.Mission> missions,
            List<UAV> uavs,
            int seed = 42
        )
        {
            var random = new Random(seed);
            var assignments = new List<AssignmentGene>();

            foreach (var mission in missions)
            {
                var compatibleUAVs = uavs.Where(u => u.UavType == mission.RequiredUAVType).ToList();
                if (compatibleUAVs.Any())
                {
                    var selectedUAV = compatibleUAVs[random.Next(compatibleUAVs.Count)];
                    assignments.Add(
                        new AssignmentGene
                        {
                            Mission = mission,
                            UAV = selectedUAV,
                            TimeWindow = new TimeWindow()
                            {
                                Start = mission.TimeWindow.Start,
                                End = mission.TimeWindow.End,
                            },
                        }
                    );
                }
            }

            return new AssignmentChromosome { Assignments = assignments };
        }
    }
}
