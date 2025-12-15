using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies
{
    public class TypeMismatchRepairStrategy : IRepairStrategy
    {
        public void RepairChromosomeViolation(
            AssignmentChromosome assignmentChromosome,
            IEnumerable<Mission> missions,
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

            IEnumerable<AssignmentGene> repairedAssignments = assignmentChromosome
                .Assignments.Select(gene => RepairGeneIfNeeded(gene, uavs))
                .Where(gene => gene != null)
                .Cast<AssignmentGene>();

            assignmentChromosome.Assignments = repairedAssignments;
        }

        private AssignmentGene RepairGeneIfNeeded(AssignmentGene gene, IEnumerable<UAV> uavs)
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
            int count = compatibleUAVs.Count();
            int randomIndex = Random.Shared.Next(count);
            return compatibleUAVs.ElementAt(randomIndex);
        }
    }
}
