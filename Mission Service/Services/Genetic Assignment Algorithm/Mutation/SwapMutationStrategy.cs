using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Mutation
{
    public class SwapMutationStrategy : IMutationStrategy
    {
        public void MutateChromosome(
            AssignmentChromosome assignmentChromosome,
            IEnumerable<UAV> uavs
        )
        {
            if (
                assignmentChromosome?.Assignments == null
                || !assignmentChromosome.Assignments.Any()
            )
            {
                return;
            }

            bool mutateUAV =
                Random.Shared.Next(MissionServiceConstants.MainAlgorithm.AMOUNT_OF_MUTATION_OPTIONS)
                == 0;

            if (mutateUAV)
            {
                MutateUAVSwap(assignmentChromosome, uavs);
            }
            else
            {
                MutateAssignmentSwap(assignmentChromosome);
            }
        }

        private void MutateUAVSwap(AssignmentChromosome assignmentChromosome, IEnumerable<UAV> uavs)
        {
            List<AssignmentGene> assignments = assignmentChromosome.Assignments.ToList();
            int randomIndex = Random.Shared.Next(assignments.Count);
            AssignmentGene geneToMutate = assignments[randomIndex];

            List<UAV> compatibleUAVs = uavs.Where(uav => IsValidUAVForSwap(uav, geneToMutate))
                .ToList();

            if (compatibleUAVs.Count > 0)
            {
                geneToMutate.UAV = compatibleUAVs[Random.Shared.Next(compatibleUAVs.Count)];
            }
        }

        private void MutateAssignmentSwap(AssignmentChromosome assignmentChromosome)
        {
            List<AssignmentGene> assignments = assignmentChromosome.Assignments.ToList();

            if (assignments.Count < 2)
            {
                return;
            }

            int firstIndex = Random.Shared.Next(assignments.Count);
            int secondIndex = Random.Shared.Next(assignments.Count);

            while (secondIndex == firstIndex && assignments.Count > 1)
            {
                secondIndex = Random.Shared.Next(assignments.Count);
            }

            (assignments[firstIndex].UAV, assignments[secondIndex].UAV) = (
                assignments[secondIndex].UAV,
                assignments[firstIndex].UAV
            );
        }

        private bool IsValidUAVForSwap(UAV uav, AssignmentGene gene)
        {
            return uav.UavType == gene.Mission.RequiredUAVType && uav.TailId != gene.UAV.TailId;
        }
    }
}
