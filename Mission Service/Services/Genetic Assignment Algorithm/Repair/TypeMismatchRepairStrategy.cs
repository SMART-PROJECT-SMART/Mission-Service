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
            for (int assignmentIndex = assignmentChromosome.Assignments.Count() - 1; assignmentIndex >= 0; assignmentIndex--)
            {
                AssignmentGene currentGene = assignmentChromosome.Assignments.ElementAt(assignmentIndex);

                if (IsTypeMismatch(currentGene))
                {
                    List<UAV> compatibleUAVs = FindCompatibleUAVs(currentGene.Mission, uavs);

                    if (compatibleUAVs.Count > 0)
                    {
                        currentGene.UAV = SelectRandomUAV(compatibleUAVs);
                    }
                    else
                    {
                        assignmentChromosome.Assignments.RemoveAt(assignmentIndex);
                    }
                }
            }
        }

        private bool IsTypeMismatch(AssignmentGene gene)
        {
            return gene.Mission.RequiredUAVType != gene.UAV.UavType;
        }

        private List<UAV> FindCompatibleUAVs(Mission mission, List<UAV> uavs)
        {
            return uavs.Where(u => u.UavType == mission.RequiredUAVType).ToList();
        }

        private UAV SelectRandomUAV(List<UAV> compatibleUAVs)
        {
            int randomIndex = Random.Shared.Next(compatibleUAVs.Count);
            return compatibleUAVs[randomIndex];
        }
    }
}