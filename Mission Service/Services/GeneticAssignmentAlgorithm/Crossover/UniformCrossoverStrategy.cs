using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover
{
    public class UniformCrossoverStrategy : ICrossoverStrategy
    {
        public CrossoverResult CrossoverChromosomes(
            AssignmentChromosome firstChromosome,
            AssignmentChromosome secondChromosome
        )
        {
            AssignmentChromosome firstChild = CreateMissionBasedChild(
                firstChromosome,
                secondChromosome
            );
            AssignmentChromosome secondChild = CreateMissionBasedChild(
                secondChromosome,
                firstChromosome
            );

            return new CrossoverResult
            {
                FirstChromosome = firstChild,
                SecondChromosome = secondChild,
            };
        }

        private AssignmentChromosome CreateMissionBasedChild(
            AssignmentChromosome primary,
            AssignmentChromosome secondary
        )
        {
            List<AssignmentGene> primaryAssignments = primary.AssignmentsList;
            List<AssignmentGene> secondaryAssignments = secondary.AssignmentsList;

            Dictionary<string, AssignmentGene> secondaryLookup = BuildMissionLookup(
                secondaryAssignments
            );

            List<AssignmentGene> childGenes = new List<AssignmentGene>(primaryAssignments.Count);
            HashSet<string> processedMissionIds = new HashSet<string>(primaryAssignments.Count);

            InheritGenesFromPrimaryParent(
                primaryAssignments,
                secondaryLookup,
                childGenes,
                processedMissionIds
            );
            InheritUniqueGenesFromSecondaryParent(
                secondaryAssignments,
                processedMissionIds,
                childGenes
            );

            return new AssignmentChromosome { Assignments = childGenes, IsValid = true };
        }

        private Dictionary<string, AssignmentGene> BuildMissionLookup(
            List<AssignmentGene> assignments
        )
        {
            Dictionary<string, AssignmentGene> lookup = new Dictionary<string, AssignmentGene>(
                assignments.Count
            );

            foreach (AssignmentGene gene in assignments)
            {
                lookup[gene.Mission.Id] = gene;
            }

            return lookup;
        }

        private void InheritGenesFromPrimaryParent(
            List<AssignmentGene> primaryAssignments,
            Dictionary<string, AssignmentGene> secondaryLookup,
            List<AssignmentGene> childGenes,
            HashSet<string> processedMissionIds
        )
        {
            foreach (AssignmentGene primaryGene in primaryAssignments)
            {
                AssignmentGene selectedGene = SelectGeneFromParents(primaryGene, secondaryLookup);

                childGenes.Add(selectedGene.Clone());
                processedMissionIds.Add(selectedGene.Mission.Id);
            }
        }

        private AssignmentGene SelectGeneFromParents(
            AssignmentGene primaryGene,
            Dictionary<string, AssignmentGene> secondaryLookup
        )
        {
            bool secondaryHasMission = secondaryLookup.TryGetValue(
                primaryGene.Mission.Id,
                out AssignmentGene? secondaryGene
            );
            bool shouldUseSecondary =
                secondaryHasMission
                && Random.Shared.NextDouble()
                    < MissionServiceConstants.Crossover.GENE_SELECTION_PROBABILITY;

            return shouldUseSecondary ? secondaryGene! : primaryGene;
        }

        private void InheritUniqueGenesFromSecondaryParent(
            List<AssignmentGene> secondaryAssignments,
            HashSet<string> processedMissionIds,
            List<AssignmentGene> childGenes
        )
        {
            foreach (AssignmentGene secondaryGene in secondaryAssignments)
            {
                if (processedMissionIds.Add(secondaryGene.Mission.Id))
                {
                    childGenes.Add(secondaryGene.Clone());
                }
            }
        }
    }
}
