using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Common.Enums;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Mutation
{
    public class SwapMutationStrategy : IMutationStrategy
    {
        private Dictionary<UAVType, List<UAV>>? _uavsByType;

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

            InitializeUAVCacheIfNeeded(uavs);

            if (ShouldPerformUAVSwap())
            {
                MutateUAVSwap(assignmentChromosome);
            }
            else
            {
                MutateAssignmentSwap(assignmentChromosome);
            }
        }

        private void MutateUAVSwap(AssignmentChromosome assignmentChromosome)
        {
            List<AssignmentGene> assignments = assignmentChromosome.AssignmentsList;
            AssignmentGene geneToMutate = SelectRandomGene(assignments);

            List<UAV> compatibleUAVs = GetCompatibleUAVsExcludingCurrent(
                geneToMutate.Mission.RequiredUAVType,
                geneToMutate.UAV.TailId
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

        private void InitializeUAVCacheIfNeeded(IEnumerable<UAV> uavs)
        {
            _uavsByType ??= GroupUAVsByType(uavs);
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
            int currentUAVTailId
        )
        {
            if (!_uavsByType!.TryGetValue(requiredType, out List<UAV>? uavsOfType))
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
