using System.Linq;
using Mission_Service.Models;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.Helpers;

public static class TimeOverlapDetector
{
    public static Dictionary<int, List<AssignmentGene>> GroupAssignmentsByUAVTailId(
        List<AssignmentGene> assignments
    )
    {
        return assignments
            .GroupBy(assignment => assignment.UAV.TailId)
            .ToDictionary(group => group.Key, group => group.ToList());
    }

    public static int CountTimeOverlaps(
        Dictionary<int, List<AssignmentGene>> assignmentsByUAVTailId
    )
    {
        int overlapCount = 0;

        foreach (List<AssignmentGene> uavAssignments in assignmentsByUAVTailId.Values)
        {
            if (uavAssignments.Count <= 1)
                continue;

            AssignmentGene[] sortedAssignments = uavAssignments
                .OrderBy(a => a.StartTime)
                .ToArray();

            for (int i = 0; i < sortedAssignments.Length - 1; i++)
            {
                if (sortedAssignments[i].EndTime > sortedAssignments[i + 1].StartTime)
                {
                    overlapCount++;
                }
            }
        }

        return overlapCount;
    }
}
