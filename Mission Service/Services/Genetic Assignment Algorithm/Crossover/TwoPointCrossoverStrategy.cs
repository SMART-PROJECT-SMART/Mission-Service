using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Crossover
{
    public class TwoPointCrossoverStrategy : ICrossoverStrategy
    {
        public CrossoverResult CrossoverChromosomes(
            AssignmentChromosome firstChromosome,
            AssignmentChromosome secondChromosome
        )
        {
            // Create children using mission-based crossover to preserve all missions
            AssignmentChromosome firstChild = CreateMissionBasedChild(firstChromosome, secondChromosome);
            AssignmentChromosome secondChild = CreateMissionBasedChild(secondChromosome, firstChromosome);

            return new CrossoverResult
            {
                FirstChromosome = firstChild,
                SecondChromosome = secondChild,
            };
        }

        private AssignmentChromosome CreateMissionBasedChild(
            AssignmentChromosome primary,
            AssignmentChromosome secondary
        )
        {
            // Get all unique missions from both parents
            Dictionary<string, AssignmentGene> primaryAssignments = primary.Assignments
                .ToDictionary(a => a.Mission.Id, a => a);

            Dictionary<string, AssignmentGene> secondaryAssignments = secondary.Assignments
                .ToDictionary(a => a.Mission.Id, a => a);

            List<AssignmentGene> childGenes = new List<AssignmentGene>();

            // For each mission in primary, randomly choose UAV assignment from primary or secondary
            foreach (var missionId in primaryAssignments.Keys)
            {
                AssignmentGene sourceGene;

                // If both parents have this mission, randomly choose which parent's UAV assignment to use
                if (secondaryAssignments.ContainsKey(missionId) && Random.Shared.NextDouble() < 0.5)
                {
                    sourceGene = secondaryAssignments[missionId];
                }
                else
                {
                    sourceGene = primaryAssignments[missionId];
                }

                // Create a copy of the gene
                AssignmentGene newGene = new AssignmentGene
                {
                    Mission = sourceGene.Mission,
                    UAV = sourceGene.UAV,
                    StartTime = sourceGene.StartTime,
                    Duration = sourceGene.Duration,
                };
                childGenes.Add(newGene);
            }

            // Add any missions from secondary that aren't in primary
            foreach (var missionId in secondaryAssignments.Keys)
            {
                if (!primaryAssignments.ContainsKey(missionId))
                {
                    var sourceGene = secondaryAssignments[missionId];
                    AssignmentGene newGene = new AssignmentGene
                    {
                        Mission = sourceGene.Mission,
                        UAV = sourceGene.UAV,
                        StartTime = sourceGene.StartTime,
                        Duration = sourceGene.Duration,
                    };
                    childGenes.Add(newGene);
                }
            }

            AssignmentChromosome childChromosome = new AssignmentChromosome
            {
                Assignments = childGenes,
                IsValid = true, // Will be validated by repair pipeline
            };

            return childChromosome;
        }
    }
}
