using Mission_Service.Models;
using System.Linq;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.Helpers;

public static class MissionAnalyzer
{
    public static HashSet<string> GetUniqueMissionIds(List<AssignmentGene> assignments)
    {
       return assignments.Select(assignment => assignment.Mission.Id).ToHashSet();
    }

    public static int CountTypeMismatches(List<AssignmentGene> assignments)
    {
        return assignments.Count(assignment => 
        assignment.Mission.RequiredUAVType != assignment.UAV.UavType);
    }

    public static double CalculateTotalPriority(List<AssignmentGene> assignments)
    {
        return assignments
            .DistinctBy(assignment => assignment.Mission.Id)
            .Sum(assignment => (int)assignment.Mission.Priority);
    }
}
