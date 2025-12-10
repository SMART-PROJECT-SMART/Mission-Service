using Mission_Service.Models;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness.Helpers;

public static class MissionAnalyzer
{
    public static HashSet<string> GetUniqueMissionIds(List<AssignmentGene> assignments)
    {
        HashSet<string> uniqueMissionIds = new HashSet<string>(assignments.Count);

        foreach (AssignmentGene assignment in assignments)
        {
            uniqueMissionIds.Add(assignment.Mission.Id);
        }

        return uniqueMissionIds;
    }

    public static int CountTypeMismatches(List<AssignmentGene> assignments)
    {
        int mismatchCount = 0;

        foreach (AssignmentGene assignment in assignments)
        {
            if (assignment.Mission.RequiredUAVType != assignment.UAV.UavType)
            {
                mismatchCount++;
            }
        }

        return mismatchCount;
    }

    public static double CalculateTotalPriority(List<AssignmentGene> assignments)
    {
        HashSet<string> seenMissionIds = new HashSet<string>(assignments.Count);
        double totalPriority = 0.0;

        foreach (AssignmentGene assignment in assignments)
        {
            if (seenMissionIds.Add(assignment.Mission.Id))
            {
                totalPriority += (int)assignment.Mission.Priority;
            }
        }

        return totalPriority;
    }
}
