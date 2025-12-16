using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Common.Helpers;
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

        private void MutateUAVSwap(
            AssignmentChromosome assignmentChromosome,
            Dictionary<UAVType, List<UAV>> uavsByType
        )
        {
            List<AssignmentGene> assignments = assignmentChromosome.AssignmentsList;
            AssignmentGene geneToMutate = SelectRandomGene(assignments);

            UAV[] compatibleUAVs = GetCompatibleUAVsExcludingCurrent(
                geneToMutate.Mission.RequiredUAVType,
                geneToMutate.UAV.TailId,
                uavsByType
            );

            if (compatibleUAVs.Length > 0)
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
            return uavs.GroupBy(uav => uav.UavType)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        private UAV[] GetCompatibleUAVsExcludingCurrent(
            UAVType requiredType,
            int currentUAVTailId,
            Dictionary<UAVType, List<UAV>> uavsByType
        )
        {
            if (!uavsByType.TryGetValue(requiredType, out List<UAV>? uavsOfType))
            {
                return Array.Empty<UAV>();
            }

            return uavsOfType.Where(uav => uav.TailId != currentUAVTailId).ToArray();
        }

        private AssignmentGene SelectRandomGene(List<AssignmentGene> assignments)
        {
            return RandomSelectionHelper.SelectRandom(assignments);
        }

        private UAV SelectRandomUAV(UAV[] uavs)
        {
            return RandomSelectionHelper.SelectRandom(uavs);
        }

        private (int firstIndex, int secondIndex) SelectTwoDifferentRandomIndices(int count)
        {
            return RandomSelectionHelper.SelectTwoDifferentIndices(count);
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
            return RandomSelectionHelper.GetRandomIndex(
                    MissionServiceConstants.MainAlgorithm.AMOUNT_OF_MUTATION_OPTIONS
                ) == 0;
        }
    }
}
