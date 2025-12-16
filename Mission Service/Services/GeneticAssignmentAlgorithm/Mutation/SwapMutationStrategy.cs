using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation
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

            Dictionary<UAVType, List<UAV>> uavsByType = GroupUAVsByType(uavs);

            if (ShouldPerformUAVSwap())
            {
                MutateUAVSwap(assignmentChromosome, uavsByType);
            }
            else
            {
                MutateAssignmentSwap(assignmentChromosome);
            }
        }

        private void MutateUAVSwap(AssignmentChromosome assignmentChromosome, Dictionary<UAVType, List<UAV>> uavsByType)
        {
            List<AssignmentGene> assignments = assignmentChromosome.AssignmentsList;
            AssignmentGene geneToMutate = SelectRandomGene(assignments);

            List<UAV> compatibleUAVs = GetCompatibleUAVsExcludingCurrent(
                geneToMutate.Mission.RequiredUAVType,
                geneToMutate.UAV.TailId,
                uavsByType
            );

            if (compatibleUAVs.Count > 0)
            {
                geneToMutate.UAV = SelectRandomUAV(compatibleUAVs);
            }
        }

        private void MutateAssignmentSwap(AssignmentChromosome assignmentChromosome)
        {
            List<AssignmentGene> assignments = assignmentChromosome.AssignmentsList;

            if (assignments.Count < 2)
            {
                return;
            }

            (int firstIndex, int secondIndex) = SelectTwoDifferentRandomIndices(assignments.Count);

            SwapUAVsBetweenAssignments(assignments[firstIndex], assignments[secondIndex]);
        }

        private Dictionary<UAVType, List<UAV>> GroupUAVsByType(IEnumerable<UAV> uavs)
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

        private List<UAV> GetCompatibleUAVsExcludingCurrent(
            UAVType requiredType,
            int currentUAVTailId,
            Dictionary<UAVType, List<UAV>> uavsByType
        )
        {
            if (!uavsByType.TryGetValue(requiredType, out List<UAV>? uavsOfType))
            {
                return new List<UAV>();
            }

            return uavsOfType.Where(uav => uav.TailId != currentUAVTailId).ToList();
        }

        private AssignmentGene SelectRandomGene(List<AssignmentGene> assignments)
        {
            int randomIndex = Random.Shared.Next(assignments.Count);
            return assignments[randomIndex];
        }

        private UAV SelectRandomUAV(List<UAV> uavs)
        {
            int randomIndex = Random.Shared.Next(uavs.Count);
            return uavs[randomIndex];
        }

        private (int firstIndex, int secondIndex) SelectTwoDifferentRandomIndices(int count)
        {
            int firstIndex = Random.Shared.Next(count);
            int secondIndex = Random.Shared.Next(count);

            while (secondIndex == firstIndex)
            {
                secondIndex = Random.Shared.Next(count);
            }

            return (firstIndex, secondIndex);
        }

        private void SwapUAVsBetweenAssignments(
            AssignmentGene firstAssignment,
            AssignmentGene secondAssignment
        )
        {
            (firstAssignment.UAV, secondAssignment.UAV) = (
                secondAssignment.UAV,
                firstAssignment.UAV
            );
        }

        private bool ShouldPerformUAVSwap()
        {
            return Random.Shared.Next(
                    MissionServiceConstants.MainAlgorithm.AMOUNT_OF_MUTATION_OPTIONS
                ) == 0;
        }
    }
}
