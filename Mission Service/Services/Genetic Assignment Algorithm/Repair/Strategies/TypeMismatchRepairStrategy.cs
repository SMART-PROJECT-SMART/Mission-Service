using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Repair
{
    public class TypeMismatchRepairStrategy : IRepairStrategy
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

      IEnumerable<AssignmentGene> repairedAssignments = assignmentChromosome.Assignments
     .Select(gene => RepairGeneIfNeeded(gene, uavs))
     .Where(gene => gene != null)
        .Cast<AssignmentGene>();

            assignmentChromosome.Assignments = repairedAssignments;
        }

        private AssignmentGene RepairGeneIfNeeded(AssignmentGene gene, List<UAV> uavs)
        {
          if (!IsTypeMismatch(gene))
     {
      return gene;
          }

      IEnumerable<UAV> compatibleUAVs = FindCompatibleUAVs(gene.Mission, uavs);

            if (compatibleUAVs.Any())
   {
    gene.UAV = SelectRandomUAV(compatibleUAVs);
     return gene;
    }

  return null;
  }

      private bool IsTypeMismatch(AssignmentGene gene)
     {
        return gene.Mission.RequiredUAVType != gene.UAV.UavType;
        }

        private IEnumerable<UAV> FindCompatibleUAVs(Mission mission, IEnumerable<UAV> uavs)
     {
     return uavs.Where(u => u.UavType == mission.RequiredUAVType);
        }

private UAV SelectRandomUAV(IEnumerable<UAV> compatibleUAVs)
 {
     List<UAV> uavList = compatibleUAVs.ToList();
      return uavList[Random.Shared.Next(uavList.Count)];
   }
    }
}