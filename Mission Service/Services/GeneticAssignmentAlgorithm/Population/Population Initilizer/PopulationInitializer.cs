using Microsoft.Extensions.Options;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Population.Population_Initilizer
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
            // Materialize collections once for reuse across all chromosomes
            IReadOnlyList<Mission> missionList = missions as IReadOnlyList<Mission> ?? missions.ToList();
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
            Dictionary<UAVType, List<UAV>> uavsByType = new Dictionary<UAVType, List<UAV>>();

            foreach (UAV uav in uavs)
            {
                if (!uavsByType.TryGetValue(uav.UavType, out List<UAV>? uavList))
                {
                    uavList = new List<UAV>();
                    uavsByType[uav.UavType] = uavList;
                }
                uavList.Add(uav);
            }

            return uavsByType;
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

                UAV selectedUav = compatibleUAVs[Random.Shared.Next(compatibleUAVs.Count)];

                assignments.Add(
                    new AssignmentGene
                    {
                        Mission = mission,
                        UAV = selectedUav,
                        StartTime = mission.TimeWindow.Start,
                        Duration = mission.TimeWindow.End - mission.TimeWindow.Start,
                    }
                );
            }

            return new AssignmentChromosome { Assignments = assignments, IsValid = true };
        }
    }
}
