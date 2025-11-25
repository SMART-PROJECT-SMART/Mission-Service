using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Population.Population_Initilizer
{
    public class PopulationInitializer : IPopulationInitializer
    {
        private readonly AssignmentAlgorithmConfiguration _algorithmConfig;

        public PopulationInitializer(
            IOptions<AssignmentAlgorithmConfiguration> algorithmConfig)
        {
            _algorithmConfig = algorithmConfig.Value;
        }

        public IEnumerable<AssignmentChromosome> CreateInitialPopulation(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            List<AssignmentChromosome> population = new List<AssignmentChromosome>();
            for (
                int chromosomeIndex = 0;
                chromosomeIndex < _algorithmConfig.PopulationSize;
                chromosomeIndex++
            )
            {
                AssignmentChromosome randomAssignmentChromosome = CreateRandomAssignmentChromosome(
                    missions,
                    uavs
                );
                population.Add(randomAssignmentChromosome);
            }

            return population;
        }

        public AssignmentChromosome CreateRandomAssignmentChromosome(
            IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs
        )
        {
            List<AssignmentGene> assignments = new List<AssignmentGene>();

            foreach (Mission mission in missions)
            {
                List<UAV> compatibleUAVs = uavs.Where(uav => uav.UavType == mission.RequiredUAVType)
                    .ToList();

                if (compatibleUAVs.Count == 0)
                    continue;

                UAV selectedUav = compatibleUAVs[Random.Shared.Next(compatibleUAVs.Count)];
                AssignmentGene assignmentGene = new AssignmentGene
                {
                    Mission = mission,
                    UAV = selectedUav,
                    StartTime = mission.TimeWindow.Start,
                    Duration = mission.TimeWindow.End - mission.TimeWindow.Start,
                };

                assignments.Add(assignmentGene);
            }

            AssignmentChromosome randomAssignmentChromosome = new AssignmentChromosome
            {
                Assignments = assignments,
                IsValid = true 
            };

            return randomAssignmentChromosome;
        }
    }
}
