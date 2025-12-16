using Microsoft.Extensions.Options;
using Mission_Service.Common.Enums;
using Mission_Service.Common.Helpers;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer
{
    public class PopulationInitializer : IPopulationInitializer
    {
        private readonly AssignmentAlgorithmConfiguration _algorithmConfig;

        public PopulationInitializer(IOptions<AssignmentAlgorithmConfiguration> algorithmConfig)
        {
            _algorithmConfig = algorithmConfig.Value;
        }

        public IEnumerable<AssignmentChromosome> CreateInitialPopulation(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            IReadOnlyList<Mission> missionList =
                missions as IReadOnlyList<Mission> ?? missions.ToList();
            IReadOnlyList<UAV> uavList = uavs as IReadOnlyList<UAV> ?? uavs.ToList();

            Dictionary<UAVType, List<UAV>> uavsByType = GroupUAVsByType(uavList);

            List<AssignmentChromosome> population = new List<AssignmentChromosome>(
                _algorithmConfig.PopulationSize
            );

            for (
                int chromosomeIndex = 0;
                chromosomeIndex < _algorithmConfig.PopulationSize;
                chromosomeIndex++
            )
            {
                AssignmentChromosome randomChromosome = CreateRandomAssignmentChromosome(
                    missionList,
                    uavsByType
                );
                population.Add(randomChromosome);
            }

            return population;
        }

        private Dictionary<UAVType, List<UAV>> GroupUAVsByType(IReadOnlyList<UAV> uavs)
        {
            return uavs
                .GroupBy(uav => uav.UavType)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        private AssignmentChromosome CreateRandomAssignmentChromosome(
            IReadOnlyList<Mission> missions,
            Dictionary<UAVType, List<UAV>> uavsByType
        )
        {
            List<AssignmentGene> assignments = new List<AssignmentGene>(missions.Count);

            foreach (Mission mission in missions)
            {
                if (!uavsByType.TryGetValue(mission.RequiredUAVType, out List<UAV>? compatibleUAVs))
                    continue;

                if (compatibleUAVs.Count == 0)
                    continue;

                UAV selectedUav = RandomSelectionHelper.SelectRandom(compatibleUAVs);
                DateTime randomizedStartTime = GenerateRandomStartTimeWithinWindow(mission);

                assignments.Add(
                    new AssignmentGene
                    {
                        Mission = mission,
                        UAV = selectedUav,
                        StartTime = randomizedStartTime,
                        Duration = mission.Duration,
                    }
                );
            }

            return new AssignmentChromosome { Assignments = assignments, IsValid = true };
        }

        private DateTime GenerateRandomStartTimeWithinWindow(Mission mission)
        {
            DateTime windowStart = mission.TimeWindow.Start;
            DateTime windowEnd = mission.TimeWindow.End;
            TimeSpan missionDuration = mission.Duration;

            DateTime latestPossibleStart = windowEnd - missionDuration;

            if (latestPossibleStart <= windowStart)
            {
                return windowStart;
            }

            return RandomSelectionHelper.SelectRandomTime(windowStart, latestPossibleStart);
        }
    }
}
