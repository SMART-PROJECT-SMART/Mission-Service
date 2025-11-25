using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Repair.Strategies
{
    public class DuplicateMissionRepairStrategy : IRepairStrategy
    {
        public void RepairChromosomeViolation(
      AssignmentChromosome assignmentChromosome,
         List<Mission> missions,
          List<UAV> uavs)
        {
    if (assignmentChromosome?.Assignments == null || !assignmentChromosome.Assignments.Any())
            {
          return;
   }

            List<AssignmentGene> assignmentList = assignmentChromosome.Assignments.ToList();
            HashSet<string> seenMissions = new HashSet<string>();
         List<AssignmentGene> uniqueAssignments = new List<AssignmentGene>();

        foreach (AssignmentGene assignment in assignmentList)
       {
     if (!seenMissions.Contains(assignment.Mission.Id))
   {
   seenMissions.Add(assignment.Mission.Id);
                    uniqueAssignments.Add(assignment);
    }
    }

 assignmentChromosome.Assignments = uniqueAssignments;
      }
    }
}
